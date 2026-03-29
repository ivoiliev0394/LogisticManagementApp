using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class PricingQuoteLine : BaseEntity
    {
        [Required]
        public Guid PricingQuoteId { get; set; }

        [ForeignKey(nameof(PricingQuoteId))]
        public PricingQuote PricingQuote { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int LineNo { get; set; } = 1;

        [Required]
        public PricingQuoteLineType LineType { get; set; } = PricingQuoteLineType.BaseRate;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineAmount { get; set; }

        [MaxLength(100)]
        public string? ReferenceCode { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
