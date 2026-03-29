using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentULD : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public Guid UldId { get; set; }

        [ForeignKey(nameof(UldId))]
        public ULD Uld { get; set; } = null!;

        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
