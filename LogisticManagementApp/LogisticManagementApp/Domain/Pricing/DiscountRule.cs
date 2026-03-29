using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class DiscountRule : BaseEntity
    {
        [Required]
        public Guid AgreementId { get; set; }

        [ForeignKey(nameof(AgreementId))]
        public Agreement Agreement { get; set; } = null!;

        public Guid? ServiceLevelId { get; set; }

        [ForeignKey(nameof(ServiceLevelId))]
        public ServiceLevel? ServiceLevel { get; set; }

        public Guid? GeoZoneId { get; set; }

        [ForeignKey(nameof(GeoZoneId))]
        public GeoZone? GeoZone { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinShipmentValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxShipmentValue { get; set; }

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
