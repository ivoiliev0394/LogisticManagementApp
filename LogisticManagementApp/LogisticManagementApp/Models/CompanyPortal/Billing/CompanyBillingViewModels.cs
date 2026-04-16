using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Billing
{
    public class ChargeListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string ChargeCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public bool IsTaxable { get; set; }
        public decimal? TaxRatePercent { get; set; }
        public string? Notes { get; set; }
    }

    public class ChargeRuleAppliedListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ChargeId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string ChargeCode { get; set; } = string.Empty;
        public string? SourceEntityType { get; set; }
        public Guid? SourceEntityId { get; set; }
        public string? RuleCode { get; set; }
        public string? RuleDescription { get; set; }
        public decimal? AppliedAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class InvoiceListItemViewModel
    {
        public Guid Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime IssueDateUtc { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class InvoiceLineListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string? ShipmentNo { get; set; }
        public string? ChargeCode { get; set; }
        public int LineNo { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TaxRatePercent { get; set; }
        public decimal LineNetAmount { get; set; }
        public decimal LineTaxAmount { get; set; }
        public decimal LineTotalAmount { get; set; }
    }

    public class PaymentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime PaymentDateUtc { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }
    }

    public class PaymentAllocationListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid InvoiceId { get; set; }
        public string PaymentInvoiceNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal AllocatedAmount { get; set; }
        public DateTime? AllocatedAtUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class CreditNoteListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string CreditNoteNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime IssueDateUtc { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class TaxRateListItemViewModel
    {
        public Guid Id { get; set; }
        public string TaxType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CountryCode { get; set; }
        public decimal RatePercent { get; set; }
        public DateTime? ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
