using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentTrip : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public Guid TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = null!;

        /// <summary>
        /// Ако искаш връзката да е към конкретен shipment leg.
        /// </summary>
        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
