using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Contacts
{
    public class CompanyContactCreateViewModel
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [EmailAddress]
        [StringLength(120)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(40)]
        public string? Phone { get; set; }

        [StringLength(120)]
        public string? RoleTitle { get; set; }

        public bool IsPrimary { get; set; }
    }
}
