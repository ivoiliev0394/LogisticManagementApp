using LogisticManagementApp.Domain.Enums.Pricing;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Pricing
{
    public class AgreementCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string AgreementNumber { get; set; } = string.Empty;

        [Required]
        public AgreementType AgreementType { get; set; } = AgreementType.CustomerAgreement;

        [Required]
        public DateTime ValidFromUtc { get; set; } = DateTime.UtcNow.Date;

        public DateTime? ValidToUtc { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [StringLength(100)]
        public string? PaymentTerms { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class AgreementEditViewModel : AgreementCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DiscountRuleCreateViewModel
    {
        [Required]
        public Guid AgreementId { get; set; }

        public Guid? ServiceLevelId { get; set; }

        public Guid? GeoZoneId { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinShipmentValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxShipmentValue { get; set; }

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> AgreementOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ServiceLevelOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> GeoZoneOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class DiscountRuleEditViewModel : DiscountRuleCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class PricingQuoteCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string QuoteNumber { get; set; } = string.Empty;

        public Guid? AgreementId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ServiceLevelId { get; set; }

        [Required]
        public PricingQuoteStatus Status { get; set; } = PricingQuoteStatus.Draft;

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "EUR";

        public DateTime? ValidUntilUtc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal NetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> AgreementOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> OrderOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ServiceLevelOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class PricingQuoteEditViewModel : PricingQuoteCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class PricingQuoteLineCreateViewModel
    {
        [Required]
        public Guid PricingQuoteId { get; set; }

        [Range(1, int.MaxValue)]
        public int LineNo { get; set; } = 1;

        [Required]
        public PricingQuoteLineType LineType { get; set; } = PricingQuoteLineType.BaseRate;

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineAmount { get; set; }

        [StringLength(100)]
        public string? ReferenceCode { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> PricingQuoteOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class PricingQuoteLineEditViewModel : PricingQuoteLineCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
