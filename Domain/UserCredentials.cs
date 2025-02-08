using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace hh_napi.Domain
{
    public class UserCredentials
    {
        [Key]
        [ForeignKey("User")]
        public required int UserId { get; set; }
        public required string PasswordHash { get; set; } = null!;
        public required string Salt { get; set; } = null!;
    }
}