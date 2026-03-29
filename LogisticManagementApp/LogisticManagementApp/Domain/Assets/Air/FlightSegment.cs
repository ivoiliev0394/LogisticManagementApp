using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Air
{
    public class FlightSegment : BaseEntity
    {
        [Required]
        public Guid FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int SegmentNo { get; set; } = 1;

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
