using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Pricing
{
    public class Surcharge : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = null!; // FUEL, DG, OVERSIZE

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public SurchargeType SurchargeType { get; set; } = SurchargeType.Fixed;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Value { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<TariffSurcharge> TariffSurcharges { get; set; } = new List<TariffSurcharge>();
    }
}
