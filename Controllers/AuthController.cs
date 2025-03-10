using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Services;
using hh_napi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILoginAttemptService _loginAttemptService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            ILoginAttemptService loginAttemptService,
            IConfiguration config,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _loginAttemptService = loginAttemptService;
            _config = config;
            _logger = logger;
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            User? user = await _userService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);
            if (user == null)
            {
                // Record failed login attempt
                await _loginAttemptService.RecordFailedAttemptAsync(loginRequest.Username);

                _logger.LogWarning("Login attempt failed for username {Username}", loginRequest.Username);
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Record successful login attempt
            await _loginAttemptService.RecordSuccessfulAttemptAsync(loginRequest.Username);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id);

            var jwtSettings = _config.GetSection("Jwt");
            var expiresInSeconds = Convert.ToInt32(jwtSettings["AccessTokenExpiryMinutes"]) * 60;

            _logger.LogInformation("Login successful for username {Username}", loginRequest.Username);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = expiresInSeconds
            });
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var (accessToken, refreshToken) = await _tokenService.RefreshTokenAsync(
                    request.AccessToken,
                    request.RefreshToken);

                var jwtSettings = _config.GetSection("Jwt");
                var expiresInSeconds = Convert.ToInt32(jwtSettings["AccessTokenExpiryMinutes"]) * 60;

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = expiresInSeconds
                });
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid token during refresh attempt");
                return Unauthorized(new { Message = "Invalid token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return BadRequest(new { Message = "Error refreshing token" });
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request)
        {
            try
            {
                await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken, "Revoked by user");
                return Ok(new { Message = "Token revoked" });
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid token during revoke attempt");
                return BadRequest(new { Message = "Invalid token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return BadRequest(new { Message = "Error revoking token" });
            }
        }
    }
}
