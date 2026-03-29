using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations
{
    public class ConsolidationShipment : BaseEntity
    {
        [Required]
        public Guid ConsolidationId { get; set; }

        [ForeignKey(nameof(ConsolidationId))]
        public Consolidation Consolidation { get; set; } = null!;

        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        /// <summary>
        /// Ако участва само конкретен leg от shipment.
        /// </summary>
        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AllocatedWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AllocatedVolumeCbm { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
