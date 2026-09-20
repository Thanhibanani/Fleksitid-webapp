namespace Fleksitid.Models
{
    public class ColleaguesViewModel
    {
        public List<ColleagueConsent> MyShares { get; set; } = new();
        public List<ColleagueConsent> SharedWithMe { get; set; } = new();
    }

    public class ColleagueTimeViewModel
    {
        public string ColleagueName { get; set; } = string.Empty;
        public decimal WeeklyContractHours { get; set; }
        public decimal WeekWorkedHours { get; set; }
        public decimal TotalFlexBalance { get; set; }
        public List<TimeEntry> RecentEntries { get; set; } = new();
        public List<SickDay> RecentSickDays { get; set; } = new();
    }
}
