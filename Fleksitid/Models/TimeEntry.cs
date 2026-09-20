using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    public enum TimeEntrySource
    {
        Stempling,
        Manuell
    }

    /// <summary>Én dags tidsregistrering: enten stemplet inn/ut, eller manuelt lagt inn.</summary>
    public class TimeEntry
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        /// <summary>Brukt når timer legges inn manuelt uten start-/sluttklokkeslett.</summary>
        public decimal? ManualHours { get; set; }

        public TimeEntrySource Source { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsOpen => Source == TimeEntrySource.Stempling && StartTime is not null && EndTime is null;

        public decimal Hours
        {
            get
            {
                if (ManualHours is not null)
                {
                    return ManualHours.Value;
                }

                if (StartTime is null || EndTime is null)
                {
                    return 0m;
                }

                var span = EndTime.Value.ToTimeSpan() - StartTime.Value.ToTimeSpan();
                if (span < TimeSpan.Zero)
                {
                    span = span.Add(TimeSpan.FromHours(24));
                }

                return (decimal)span.TotalHours;
            }
        }
    }
}
