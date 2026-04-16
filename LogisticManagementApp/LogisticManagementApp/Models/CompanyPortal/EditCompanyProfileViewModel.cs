using LogisticManagementApp.Domain.Enums.Companies;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal
{
    public class EditCompanyProfileViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [Required]
        public CompanyType CompanyType { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
