
using System.Security.Claims;
using hh_napi.Domain;

namespace hh_napi.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    Task<RefreshToken> CreateRefreshTokenAsync(int userId);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<(string accessToken, RefreshToken refreshToken)> RefreshTokenAsync(string accessToken, string refreshTokenString);
    Task RevokeRefreshTokenAsync(string token, string reason);
}