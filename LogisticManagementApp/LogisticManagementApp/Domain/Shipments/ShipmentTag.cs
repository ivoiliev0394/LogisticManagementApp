using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentTag : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public ShipmentTagType TagType { get; set; } = ShipmentTagType.Other;

        [MaxLength(100)]
        public string? CustomValue { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
