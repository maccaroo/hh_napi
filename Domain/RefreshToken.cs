using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hh_napi.Domain
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        public bool IsRevoked { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? ReplacedByToken { get; set; }
        
        public string? RevokedReason { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
