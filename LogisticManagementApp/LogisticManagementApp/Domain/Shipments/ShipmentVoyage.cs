using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentVoyage : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public Guid VoyageId { get; set; }

        [ForeignKey(nameof(VoyageId))]
        public Voyage Voyage { get; set; } = null!;

        /// <summary>
        /// Ако конкретният shipment е вързан към определен leg.
        /// </summary>
        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [MaxLength(100)]
        public string? BookingReference { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
