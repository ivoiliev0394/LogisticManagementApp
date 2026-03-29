using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Sea
{
    public class Voyage : BaseEntity
    {
        [Required]
        public Guid VesselId { get; set; }

        [ForeignKey(nameof(VesselId))]
        public Vessel Vessel { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string VoyageNumber { get; set; } = null!;

        [Required]
        public TripStatus Status { get; set; } = TripStatus.Planned;

        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [Required]
        [MaxLength(100)]
        public string OriginPort { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string DestinationPort { get; set; } = null!;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<VoyageStop> Stops { get; set; } = new List<VoyageStop>();
    }
}
