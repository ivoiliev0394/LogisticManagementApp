using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentLeg : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int LegNo { get; set; } = 1;

        [Required]
        public TransportMode Mode { get; set; } = TransportMode.Road;

        [Required]
        public Guid OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location OriginLocation { get; set; } = null!;

        [Required]
        public Guid DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location DestinationLocation { get; set; } = null!;

        [Required]
        public LegStatus Status { get; set; } = LegStatus.Planned;

        /// <summary>
        /// Планирани и реални времена.
        /// </summary>
        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }
        public DateTime? ATD_Utc { get; set; }
        public DateTime? ATA_Utc { get; set; }

        [MaxLength(100)]
        public string? CarrierReference { get; set; } // Voyage/Flight/Trip ref (като текст за момента)

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
