using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations
{
    public class BookingLeg : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        /// <summary>
        /// Ако leg-ът от booking е вързан към реален shipment leg.
        /// </summary>
        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LegNo { get; set; } = 1;

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }

        [MaxLength(100)]
        public string? CarrierReference { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
