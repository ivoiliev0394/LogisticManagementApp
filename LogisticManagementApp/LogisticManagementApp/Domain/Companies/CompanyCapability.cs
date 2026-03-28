using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Companies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Companies
{
    public class CompanyCapability : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        public CompanyCapabilityType CapabilityType { get; set; }

        [Required]
        public bool IsEnabled { get; set; } = true;

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
