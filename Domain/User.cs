using hh_napi.Attributes;

namespace hh_napi.Domain
{
    public class User
    {
        public int Id { get; set; }

        [Searchable]
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [DefaultOrderBy]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}