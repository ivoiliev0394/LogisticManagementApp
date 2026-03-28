using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Companies
{
    public class CompanyContact : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [MaxLength(120)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(40)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(120)]
        public string? RoleTitle { get; set; } // напр. Dispatch, Sales, Operations

        [Required]
        public bool IsPrimary { get; set; } = false;
    }
}
