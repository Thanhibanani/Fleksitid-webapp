using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    public class SickDay
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
