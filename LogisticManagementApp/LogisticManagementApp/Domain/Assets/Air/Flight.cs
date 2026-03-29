using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Air
{
    public class Flight : BaseEntity
    {
        [Required]
        public Guid AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft Aircraft { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = null!;

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        [Required]
        public FlightStatus Status { get; set; } = FlightStatus.Planned;

        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<FlightSegment> Segments { get; set; } = new List<FlightSegment>();
        public ICollection<AirCrewAssignment> CrewAssignments { get; set; } = new List<AirCrewAssignment>();
    }
}
