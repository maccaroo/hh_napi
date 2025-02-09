namespace hh_napi.Models.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}