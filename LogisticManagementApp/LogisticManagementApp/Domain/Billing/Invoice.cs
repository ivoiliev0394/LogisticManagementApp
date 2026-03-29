using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Billing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class Invoice : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string InvoiceNo { get; set; } = null!;

        [Required]
        public Guid BillToCompanyId { get; set; }

        [ForeignKey(nameof(BillToCompanyId))]
        public Company BillToCompany { get; set; } = null!;

        [Required]
        public DateTime IssueDateUtc { get; set; } = DateTime.UtcNow;

        public DateTime? DueDateUtc { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        [Range(0, double.MaxValue)]
        public decimal SubtotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
