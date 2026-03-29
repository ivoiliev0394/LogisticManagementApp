using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Billing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class Payment : BaseEntity
    {
        [Required]
        public Guid InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;

        [Required]
        public DateTime PaymentDateUtc { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BankTransfer;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
