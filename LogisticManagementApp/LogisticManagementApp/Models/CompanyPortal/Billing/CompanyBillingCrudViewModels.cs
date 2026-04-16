using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Billing
{
    public class ChargeCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public Guid? ShipmentLegId { get; set; }

        [Required, StringLength(30)]
        public string ChargeCode { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public ChargeSourceType SourceType { get; set; } = ChargeSourceType.Manual;

        public bool IsTaxable { get; set; } = true;

        [Range(0, 100)]
        public decimal? TaxRatePercent { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class ChargeEditViewModel : ChargeCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class ChargeRuleAppliedCreateViewModel
    {
        [Required]
        public Guid ChargeId { get; set; }

        [StringLength(100)]
        public string? SourceEntityType { get; set; }

        public Guid? SourceEntityId { get; set; }

        [StringLength(100)]
        public string? RuleCode { get; set; }

        [StringLength(300)]
        public string? RuleDescription { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AppliedAmount { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> ChargeOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class ChargeRuleAppliedEditViewModel : ChargeRuleAppliedCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class InvoiceCreateViewModel
    {
        [Required, StringLength(30)]
        public string InvoiceNo { get; set; } = string.Empty;

        [Required]
        public DateTime IssueDateUtc { get; set; } = DateTime.UtcNow.Date;

        public DateTime? DueDateUtc { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        [Range(0, double.MaxValue)]
        public decimal SubtotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class InvoiceEditViewModel : InvoiceCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class InvoiceLineCreateViewModel
    {
        [Required]
        public Guid InvoiceId { get; set; }

        public Guid? ChargeId { get; set; }
        public Guid? ShipmentId { get; set; }

        [Range(1, int.MaxValue)]
        public int LineNo { get; set; } = 1;

        [Required, StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal? TaxRatePercent { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineNetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineTaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LineTotalAmount { get; set; }

        public IEnumerable<SelectListItem> InvoiceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ChargeOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class InvoiceLineEditViewModel : InvoiceLineCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class PaymentCreateViewModel
    {
        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        public DateTime PaymentDateUtc { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BankTransfer;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> InvoiceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class PaymentEditViewModel : PaymentCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class PaymentAllocationCreateViewModel
    {
        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        public Guid InvoiceId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AllocatedAmount { get; set; }

        public DateTime? AllocatedAtUtc { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> PaymentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> InvoiceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class PaymentAllocationEditViewModel : PaymentAllocationCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class CreditNoteCreateViewModel
    {
        [Required, StringLength(30)]
        public string CreditNoteNo { get; set; } = string.Empty;

        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        public DateTime IssueDateUtc { get; set; } = DateTime.UtcNow.Date;

        [Required, StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public CreditNoteStatus Status { get; set; } = CreditNoteStatus.Draft;

        [Range(0, double.MaxValue)]
        public decimal NetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> InvoiceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }

    public class CreditNoteEditViewModel : CreditNoteCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
