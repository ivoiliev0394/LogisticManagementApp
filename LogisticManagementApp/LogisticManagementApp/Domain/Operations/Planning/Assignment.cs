using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Planning
{
    public class Assignment : BaseEntity
    {
        [Required]
        public Guid ShipmentLegId { get; set; }

        [ForeignKey(nameof(ShipmentLegId))]
        public ShipmentLeg ShipmentLeg { get; set; } = null!;

        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Planned;

        public DateTime? AssignedAtUtc { get; set; }

        [MaxLength(100)]
        public string? ReferenceNo { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
