using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    public class ConsultantOverview
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal WeeklyContractHours { get; set; }
        public decimal WeekWorkedHours { get; set; }
        public decimal TotalFlexBalance { get; set; }
        public int SickDaysThisYear { get; set; }
        public bool IsClockedIn { get; set; }
    }

    public class DashboardViewModel
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string OrgCode { get; set; } = string.Empty;
        public List<ConsultantOverview> Consultants { get; set; } = new();
    }

    public class ContractInput
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Range(0, 60)]
        public decimal WeeklyHours { get; set; }

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }

    public class ConsultantDetailsViewModel
    {
        public ConsultantOverview Consultant { get; set; } = new();
        public List<WorkContract> Contracts { get; set; } = new();
        public List<TimeEntry> Entries { get; set; } = new();
        public List<SickDay> SickDays { get; set; } = new();
        public ContractInput NewContract { get; set; } = new();
    }
}
