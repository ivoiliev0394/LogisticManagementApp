using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Compliance;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Compliance
{
    public class DangerousGoodsDeclaration : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        public Guid? PackageId { get; set; }

        [ForeignKey(nameof(PackageId))]
        public Package? Package { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnNumber { get; set; } = null!; // UN 1203

        [Required]
        [MaxLength(200)]
        public string ProperShippingName { get; set; } = null!;

        [Required]
        public HazardClass HazardClass { get; set; }

        public PackingGroup? PackingGroup { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? NetQuantity { get; set; }

        [MaxLength(50)]
        public string? QuantityUnit { get; set; }

        [MaxLength(500)]
        public string? HandlingInstructions { get; set; }

        [Required]
        public bool RequiresSpecialHandling { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
