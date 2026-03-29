using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class TariffSurcharge : BaseEntity
    {
        [Required]
        public Guid TariffId { get; set; }

        [ForeignKey(nameof(TariffId))]
        public Tariff Tariff { get; set; } = null!;

        [Required]
        public Guid SurchargeId { get; set; }

        [ForeignKey(nameof(SurchargeId))]
        public Surcharge Surcharge { get; set; } = null!;

        [Required]
        public SurchargeType ApplyAs { get; set; } = SurchargeType.Fixed;

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxAmount { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
