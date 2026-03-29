using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Billing;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Billing
{
    public class Charge : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        public Guid? ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg? ShipmentLeg { get; set; }

        [Required]
        [MaxLength(30)]
        public string ChargeCode { get; set; } = null!; // BASE, FUEL, HANDLING

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public ChargeSourceType SourceType { get; set; } = ChargeSourceType.Manual;

        [Required]
        public bool IsTaxable { get; set; } = true;

        [Range(0, 100)]
        public decimal? TaxRatePercent { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }

        /// <summary>
        /// Ако е фактурирана, ще се върже чрез InvoiceLine.
        /// </summary>
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
