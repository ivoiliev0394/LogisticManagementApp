using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentReference : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public ShipmentReferenceType ReferenceType { get; set; } = ShipmentReferenceType.Other;

        [Required]
        [MaxLength(100)]
        public string ReferenceValue { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
