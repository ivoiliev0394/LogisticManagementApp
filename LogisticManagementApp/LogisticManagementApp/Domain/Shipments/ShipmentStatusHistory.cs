using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ShipmentStatusHistory : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public ShipmentStatus OldStatus { get; set; }

        [Required]
        public ShipmentStatus NewStatus { get; set; }

        [Required]
        public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
