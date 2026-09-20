using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    public class ManualEntryInput
    {
        [Required(ErrorMessage = "Du må velge en dato.")]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(0.25, 24, ErrorMessage = "Antall timer må være mellom 0,25 og 24.")]
        public decimal Hours { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class SickDayInput
    {
        [Required(ErrorMessage = "Du må velge en fra-dato.")]
        [DataType(DataType.Date)]
        public DateOnly FromDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "Du må velge en til-dato.")]
        [DataType(DataType.Date)]
        public DateOnly ToDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class TidsregistreringViewModel
    {
        public TimeEntry? OpenEntry { get; set; }
        public decimal WeeklyContractHours { get; set; }
        public decimal WeekWorkedHours { get; set; }
        public decimal WeekFlexBalance => WeekWorkedHours - WeeklyContractHours;
        public decimal TotalFlexBalance { get; set; }
        public List<TimeEntry> RecentEntries { get; set; } = new();
        public List<SickDay> RecentSickDays { get; set; } = new();
        public ManualEntryInput ManualEntry { get; set; } = new();
        public SickDayInput SickDayEntry { get; set; } = new();
    }
}
