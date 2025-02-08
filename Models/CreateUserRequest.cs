namespace hh_napi.Models
{
    public class CreateUserRequest
    {
        public required string Username { get; set; } = null!;
        public required string Password { get; set; } = null!;
        public required string Email { get; set; } = null!;
    }
}