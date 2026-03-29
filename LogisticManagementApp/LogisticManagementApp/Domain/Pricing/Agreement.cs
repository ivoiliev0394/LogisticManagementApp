using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class Agreement : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string AgreementNumber { get; set; } = null!;

        [Required]
        public AgreementType AgreementType { get; set; } = AgreementType.CustomerAgreement;

        [Required]
        public DateTime ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [MaxLength(100)]
        public string? PaymentTerms { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<DiscountRule> DiscountRules { get; set; } = new List<DiscountRule>();
        public ICollection<PricingQuote> PricingQuotes { get; set; } = new List<PricingQuote>();
    }
}
