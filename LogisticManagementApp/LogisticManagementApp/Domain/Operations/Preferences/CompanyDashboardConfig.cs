using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Preferences
{
    public class CompanyDashboardConfig : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string DashboardKey { get; set; } = null!; // Main, Operations, Billing

        [Required]
        [MaxLength(4000)]
        public string LayoutJson { get; set; } = null!;

        [MaxLength(4000)]
        public string? WidgetSettingsJson { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
