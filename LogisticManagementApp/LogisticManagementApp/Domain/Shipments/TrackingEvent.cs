using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class TrackingEvent : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public TrackingEventType EventType { get; set; }

        [Required]
        public DateTime EventTimeUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Къде се е случило (порт, хъб, склад...) – по желание.
        /// </summary>
        public Guid? LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location? Location { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        /// <summary>
        /// Източник на събитието: UI, API, CarrierFeed, ScanDevice...
        /// </summary>
        [MaxLength(50)]
        public string? Source { get; set; }
    }
}
