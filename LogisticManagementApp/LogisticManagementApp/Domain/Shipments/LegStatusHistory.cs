using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class LegStatusHistory : BaseEntity
    {
        [Required]
        public Guid ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg ShipmentLeg { get; set; } = null!;

        [Required]
        public LegStatus OldStatus { get; set; }

        [Required]
        public LegStatus NewStatus { get; set; }

        [Required]
        public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
