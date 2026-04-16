using LogisticManagementApp.Domain.Enums.Companies;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Capabilities
{
    public class CompanyCapabilityCreateViewModel
    {
        [Required]
        public CompanyCapabilityType CapabilityType { get; set; }

        public bool IsEnabled { get; set; } = true;

        [DataType(DataType.DateTime)]
        public DateTime? ValidFromUtc { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ValidToUtc { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }
    }
}
