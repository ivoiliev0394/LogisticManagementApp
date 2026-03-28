using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Routes
{
    public class RoutePlanShipment : BaseEntity
    {
        [Required]
        public Guid RoutePlanId { get; set; }

        [ForeignKey(nameof(RoutePlanId))]
        public RoutePlan RoutePlan { get; set; } = null!;

        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        public Guid? PickupStopId { get; set; }

        [ForeignKey(nameof(PickupStopId))]
        public RoutePlanStop? PickupStop { get; set; }

        public Guid? DeliveryStopId { get; set; }

        [ForeignKey(nameof(DeliveryStopId))]
        public RoutePlanStop? DeliveryStop { get; set; }

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 1;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
