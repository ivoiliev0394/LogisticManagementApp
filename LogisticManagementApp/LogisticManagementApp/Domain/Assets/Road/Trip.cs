using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Road
{
    public class Trip : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string TripNo { get; set; } = null!;

        public Guid? VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }

        public Guid? DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        [Required]
        public TripStatus Status { get; set; } = TripStatus.Planned;

        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
