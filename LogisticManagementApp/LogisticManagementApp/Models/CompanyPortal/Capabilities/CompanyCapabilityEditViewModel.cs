using LogisticManagementApp.Domain.Enums.Companies;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Capabilities
{
    public class CompanyCapabilityEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public CompanyCapabilityType CapabilityType { get; set; }

        public bool IsEnabled { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ValidFromUtc { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ValidToUtc { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
