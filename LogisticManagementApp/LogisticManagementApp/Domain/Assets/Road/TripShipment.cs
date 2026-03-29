using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Road
{
    public class TripShipment : BaseEntity
    {
        [Required]
        public Guid TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = null!;

        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        public Guid? PickupTripStopId { get; set; }

        [ForeignKey(nameof(PickupTripStopId))]
        public TripStop? PickupTripStop { get; set; }

        public Guid? DeliveryTripStopId { get; set; }

        [ForeignKey(nameof(DeliveryTripStopId))]
        public TripStop? DeliveryTripStop { get; set; }

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 1;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
