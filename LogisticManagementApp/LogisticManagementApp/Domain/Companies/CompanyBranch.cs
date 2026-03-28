using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Companies
{
    public class CompanyBranch : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        public Guid? AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public Address? Address { get; set; }

        [MaxLength(50)]
        public string? BranchCode { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(120)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public bool IsHeadOffice { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
