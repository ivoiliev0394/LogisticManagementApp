using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Billing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class CreditNote : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string CreditNoteNo { get; set; } = null!;

        [Required]
        public Guid InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;

        [Required]
        public Guid BillToCompanyId { get; set; }

        [ForeignKey(nameof(BillToCompanyId))]
        public Company BillToCompany { get; set; } = null!;

        [Required]
        public DateTime IssueDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public CreditNoteStatus Status { get; set; } = CreditNoteStatus.Draft;

        [Range(0, double.MaxValue)]
        public decimal NetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
