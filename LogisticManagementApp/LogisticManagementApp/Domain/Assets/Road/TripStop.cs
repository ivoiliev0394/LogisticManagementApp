using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Road
{
    public class TripStop : BaseEntity
    {
        [Required]
        public Guid TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } = null!;

        [Required]
        public Guid LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int SequenceNumber { get; set; } = 1;

        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
