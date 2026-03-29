using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Pricing;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class PricingQuote : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string QuoteNumber { get; set; } = null!;

        [Required]
        public Guid CustomerCompanyId { get; set; }

        [ForeignKey(nameof(CustomerCompanyId))]
        public Company CustomerCompany { get; set; } = null!;

        public Guid? AgreementId { get; set; }

        [ForeignKey(nameof(AgreementId))]
        public Agreement? Agreement { get; set; }

        public Guid? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        public Guid? ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment? Shipment { get; set; }

        public Guid? ServiceLevelId { get; set; }

        [ForeignKey(nameof(ServiceLevelId))]
        public ServiceLevel? ServiceLevel { get; set; }

        [Required]
        public PricingQuoteStatus Status { get; set; } = PricingQuoteStatus.Draft;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        public DateTime? ValidUntilUtc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal NetAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<PricingQuoteLine> Lines { get; set; } = new List<PricingQuoteLine>();
    }
}
