using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Pricing
{
    public class ServiceLevelListItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ServiceLevelType { get; set; } = string.Empty;
        public string TransportMode { get; set; } = string.Empty;
        public decimal? MaxWeightKg { get; set; }
        public int? EstimatedTransitDays { get; set; }
        public bool IsActive { get; set; }
    }

    public class GeoZoneListItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ZoneRuleListItemViewModel
    {
        public Guid Id { get; set; }
        public string GeoZoneCode { get; set; } = string.Empty;
        public string GeoZoneName { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PostalCodeFrom { get; set; }
        public string? PostalCodeTo { get; set; }
        public int Priority { get; set; }
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
    }

    public class TariffListItemViewModel
    {
        public Guid Id { get; set; }
        public string ServiceLevelCode { get; set; } = string.Empty;
        public string GeoZoneCode { get; set; } = string.Empty;
        public string CalcBasis { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
        public bool IsActive { get; set; }
    }

    public class TariffRateListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid TariffId { get; set; }
        public string ServiceLevelCode { get; set; } = string.Empty;
        public string GeoZoneCode { get; set; } = string.Empty;
        public decimal FromValue { get; set; }
        public decimal? ToValue { get; set; }
        public decimal Price { get; set; }
        public decimal? MinCharge { get; set; }
        public decimal? StepValue { get; set; }
        public int SortOrder { get; set; }
    }

    public class SurchargeListItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SurchargeType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class TariffSurchargeListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid TariffId { get; set; }
        public string ServiceLevelCode { get; set; } = string.Empty;
        public string GeoZoneCode { get; set; } = string.Empty;
        public string SurchargeCode { get; set; } = string.Empty;
        public string ApplyAs { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool IsActive { get; set; }
    }

    public class AgreementListItemViewModel
    {
        public Guid Id { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string AgreementType { get; set; } = string.Empty;
        public DateTime ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? PaymentTerms { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class DiscountRuleListItemViewModel
    {
        public Guid Id { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string? ServiceLevelCode { get; set; }
        public string? GeoZoneCode { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal? MinShipmentValue { get; set; }
        public decimal? MaxShipmentValue { get; set; }
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class PricingQuoteListItemViewModel
    {
        public Guid Id { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public string? AgreementNumber { get; set; }
        public string? ServiceLevelCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public DateTime? ValidUntilUtc { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class PricingQuoteLineListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid PricingQuoteId { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public int LineNo { get; set; }
        public string LineType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineAmount { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Notes { get; set; }
    }
}
