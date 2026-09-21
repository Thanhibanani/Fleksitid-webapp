namespace Fleksitid.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string? Note { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}