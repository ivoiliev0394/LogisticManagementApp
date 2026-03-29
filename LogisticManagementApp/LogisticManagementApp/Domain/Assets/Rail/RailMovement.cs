using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Rail
{
    public class RailMovement : BaseEntity
    {
        public Guid? TrainId { get; set; }

        [ForeignKey(nameof(TrainId))]
        public Train? Train { get; set; }

        public Guid? RailServiceId { get; set; }

        [ForeignKey(nameof(RailServiceId))]
        public RailService? RailService { get; set; }

        [Required]
        [MaxLength(50)]
        public string MovementNo { get; set; } = null!;

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        [Required]
        public RailMovementStatus Status { get; set; } = RailMovementStatus.Planned;

        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
