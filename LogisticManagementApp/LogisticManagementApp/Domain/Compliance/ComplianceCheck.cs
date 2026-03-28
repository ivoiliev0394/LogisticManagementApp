using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Compliance;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Compliance
{
    public class ComplianceCheck : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string CheckType { get; set; } = null!; // Customs, DG, Insurance, ExportControl

        [Required]
        public ComplianceCheckStatus Status { get; set; } = ComplianceCheckStatus.Pending;

        public DateTime? CheckedAtUtc { get; set; }

        [MaxLength(100)]
        public string? CheckedBy { get; set; }

        [MaxLength(500)]
        public string? ResultDetails { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
