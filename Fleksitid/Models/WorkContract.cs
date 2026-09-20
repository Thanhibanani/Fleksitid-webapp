using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    /// <summary>Avtalt arbeidstid for en konsulent, brukt til å regne ut fleksisaldo.</summary>
    public class WorkContract
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Range(0, 60)]
        public decimal WeeklyHours { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool IsActiveOn(DateOnly date) =>
            date >= StartDate && (EndDate is null || date <= EndDate);
    }
}
