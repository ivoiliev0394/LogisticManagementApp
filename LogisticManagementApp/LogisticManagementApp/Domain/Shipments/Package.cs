using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class Package : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string PackageNo { get; set; } = null!; // PKG-0001 или barcode

        [Required]
        public PackageType PackageType { get; set; } = PackageType.Box;

        [Range(0, double.MaxValue)]
        public decimal WeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LengthCm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? WidthCm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HeightCm { get; set; }

        /// <summary>
        /// По желание: обем (CBM). Може да се изчислява, но го пазим за бързи справки.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }

        // Navigation
        public ICollection<PackageItem> Items { get; set; } = new List<PackageItem>();
    }
}
