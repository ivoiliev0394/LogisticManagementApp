using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Planning
{
    public class CapacityReservation : BaseEntity
    {
        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        public Guid? ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment? Shipment { get; set; }

        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ReservedWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ReservedVolumeCbm { get; set; }

        [Range(0, int.MaxValue)]
        public int? ReservedUnitCount { get; set; }

        public DateTime? ReservedFromUtc { get; set; }

        public DateTime? ReservedToUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
