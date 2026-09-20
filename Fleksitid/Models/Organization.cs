using System.ComponentModel.DataAnnotations;

namespace Fleksitid.Models
{
    /// <summary>A company/virksomhet. Consultants join it using its OrgCode.</summary>
    public class Organization
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string OrgCode { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
