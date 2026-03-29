using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class ZoneRule : BaseEntity
    {
        [Required]
        public Guid GeoZoneId { get; set; }

        [ForeignKey(nameof(GeoZoneId))]
        public GeoZone GeoZone { get; set; } = null!;

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(20)]
        public string? PostalCodeFrom { get; set; }

        [MaxLength(20)]
        public string? PostalCodeTo { get; set; }

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 1;

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }
    }
}
