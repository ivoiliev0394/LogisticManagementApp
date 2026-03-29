using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Billing;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Billing
{
    public class TaxRate : BaseEntity
    {
        [Required]
        public TaxType TaxType { get; set; } = TaxType.Vat;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(10)]
        public string? CountryCode { get; set; }

        [Range(0, 100)]
        public decimal RatePercent { get; set; }

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
