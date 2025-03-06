namespace hh_napi.Models
{
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }

    public class RevokeTokenRequest
    {
        public required string RefreshToken { get; set; }
    }

    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
