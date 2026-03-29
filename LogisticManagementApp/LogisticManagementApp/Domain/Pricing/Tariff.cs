using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class Tariff : BaseEntity
    {
        [Required]
        public Guid ServiceLevelId { get; set; }

        [ForeignKey(nameof(ServiceLevelId))]
        public ServiceLevel ServiceLevel { get; set; } = null!;

        [Required]
        public Guid GeoZoneId { get; set; }

        [ForeignKey(nameof(GeoZoneId))]
        public GeoZone GeoZone { get; set; } = null!;

        [Required]
        public CalcBasis CalcBasis { get; set; } = CalcBasis.Weight;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        public DateTime ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<TariffRate> Rates { get; set; } = new List<TariffRate>();
        public ICollection<TariffSurcharge> TariffSurcharges { get; set; } = new List<TariffSurcharge>();
    }
}
