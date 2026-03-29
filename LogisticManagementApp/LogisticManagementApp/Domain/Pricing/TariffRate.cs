using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Pricing
{
    public class TariffRate : BaseEntity
    {
        [Required]
        public Guid TariffId { get; set; }

        [ForeignKey(nameof(TariffId))]
        public Tariff Tariff { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal FromValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ToValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinCharge { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? StepValue { get; set; }

        [Range(1, int.MaxValue)]
        public int SortOrder { get; set; } = 1;
    }
}
