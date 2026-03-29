using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class ChargeRuleApplied : BaseEntity
    {
        [Required]
        public Guid ChargeId { get; set; }

        [ForeignKey(nameof(ChargeId))]
        public Charge Charge { get; set; } = null!;

        [MaxLength(100)]
        public string? SourceEntityType { get; set; } // Tariff, TariffRate, Surcharge, DiscountRule

        public Guid? SourceEntityId { get; set; }

        [MaxLength(100)]
        public string? RuleCode { get; set; }

        [MaxLength(300)]
        public string? RuleDescription { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AppliedAmount { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
