using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace hh_napi.Services;


public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var key = jwtSettings["Key"];
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("JWT key is missing from the configuration.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("userId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpiryMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(int userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = GenerateRefreshTokenString(),
            ExpiryDate = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration.GetSection("JwtSettings")["RefreshTokenExpiryDays"])),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return refreshToken;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var key = jwtSettings["Key"];
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("JWT key is missing from the configuration.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = false, // We don't care about the token's expiration date
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public async Task<(string accessToken, RefreshToken refreshToken)> RefreshTokenAsync(string accessToken, string refreshTokenString)
    {
        // Validate the expired access token
        var principal = GetPrincipalFromExpiredToken(accessToken);
        var userIdClaim = principal.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new SecurityTokenException("Invalid access token");
        }

        // Find the refresh token in the database
        var refreshToken = await _unitOfWork.RefreshTokens.AsQueryable()
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);

        if (refreshToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (refreshToken.UserId != userId)
        {
            throw new SecurityTokenException("Refresh token does not match the access token");
        }

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Refresh token has expired");
        }

        if (refreshToken.IsRevoked)
        {
            throw new SecurityTokenException("Refresh token has been revoked");
        }

        // Get the user
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }

        // Generate a new access token
        var newAccessToken = GenerateAccessToken(user);

        // Generate a new refresh token (token rotation for security)
        var newRefreshToken = await CreateRefreshTokenAsync(userId);

        // Revoke the old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        refreshToken.RevokedReason = "Replaced by new token";
        _unitOfWork.RefreshTokens.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string token, string reason)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.AsQueryable()
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (refreshToken.IsRevoked)
        {
            return; // Already revoked
        }

        refreshToken.IsRevoked = true;
        refreshToken.RevokedReason = reason;
        _unitOfWork.RefreshTokens.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync();
    }

    private string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
