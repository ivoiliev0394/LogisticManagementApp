using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Rail
{
    public class RailService : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string ServiceCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        public Guid? OriginLocationId { get; set; }

        [ForeignKey(nameof(OriginLocationId))]
        public Location? OriginLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }

        [ForeignKey(nameof(DestinationLocationId))]
        public Location? DestinationLocation { get; set; }

        [Required]
        public TransportMode TransportMode { get; set; } = TransportMode.Rail;

        [Range(0, int.MaxValue)]
        public int? EstimatedTransitDays { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<RailMovement> RailMovements { get; set; } = new List<RailMovement>();
    }
}
