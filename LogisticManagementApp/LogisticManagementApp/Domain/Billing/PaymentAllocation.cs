using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class PaymentAllocation : BaseEntity
    {
        [Required]
        public Guid PaymentId { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; } = null!;

        [Required]
        public Guid InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal AllocatedAmount { get; set; }

        public DateTime? AllocatedAtUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
