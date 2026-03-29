using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class InvoiceLine : BaseEntity
    {
        [Required]
        public Guid InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;

        public Guid? ChargeId { get; set; }

        [ForeignKey(nameof(ChargeId))]
        public Charge? Charge { get; set; }

        public Guid? ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment? Shipment { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LineNo { get; set; } = 1;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

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
    }
}
