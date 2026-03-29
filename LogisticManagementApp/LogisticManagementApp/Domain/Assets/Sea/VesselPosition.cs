using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Sea
{
    public class VesselPosition : BaseEntity
    {
        [Required]
        public Guid VesselId { get; set; }

        [ForeignKey(nameof(VesselId))]
        public Vessel Vessel { get; set; } = null!;

        [Required]
        public DateTime PositionTimeUtc { get; set; } = DateTime.UtcNow;

        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SpeedKnots { get; set; }

        [Range(0, 360)]
        public decimal? CourseDegrees { get; set; }

        [MaxLength(200)]
        public string? Source { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
