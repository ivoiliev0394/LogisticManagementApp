using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Compliance
{
    public class TemperatureRequirement : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Range(-100, 100)]
        public decimal? MinTemperatureCelsius { get; set; }

        [Range(-100, 100)]
        public decimal? MaxTemperatureCelsius { get; set; }

        [Required]
        public bool RequiresTemperatureMonitoring { get; set; } = true;

        [MaxLength(100)]
        public string? TemperatureUnit { get; set; } = "Celsius";

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
