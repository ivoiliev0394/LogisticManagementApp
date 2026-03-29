using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Pricing
{
    public class GeoZone : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = null!; // A, B, C, INT-1

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<ZoneRule> ZoneRules { get; set; } = new List<ZoneRule>();
        public ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
    }
}
