using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class CargoItem : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [Required]
        public CargoItemType CargoItemType { get; set; } = CargoItemType.GeneralCargo;

        [Range(0, double.MaxValue)]
        public decimal? Quantity { get; set; }

        [MaxLength(30)]
        public string? UnitOfMeasure { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? NetWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LengthCm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? WidthCm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HeightCm { get; set; }

        [MaxLength(20)]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        public string? OriginCountry { get; set; }

        [Required]
        public bool IsStackable { get; set; } = true;

        [Required]
        public bool IsFragile { get; set; } = false;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
