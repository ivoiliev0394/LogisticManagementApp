using LogisticManagementApp.Domain.Assets.CargoUnits;
using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentContainer : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public Guid ContainerId { get; set; }

        [ForeignKey(nameof(ContainerId))]
        public Container Container { get; set; } = null!;

        /// <summary>
        /// Ако контейнерът е използван за конкретен leg.
        /// </summary>
        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? GrossWeightKg { get; set; }

        [MaxLength(50)]
        public string? SealNumber { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
