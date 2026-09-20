using Microsoft.AspNetCore.Identity;

namespace Fleksitid.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public ICollection<WorkContract> WorkContracts { get; set; } = new List<WorkContract>();
        public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
        public ICollection<SickDay> SickDays { get; set; } = new List<SickDay>();
    }
}
