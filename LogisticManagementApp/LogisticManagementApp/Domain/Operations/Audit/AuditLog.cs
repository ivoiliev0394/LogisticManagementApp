using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Audit
{
    public class AuditLog : BaseEntity
    {
        [Required]
        public AuditActionType ActionType { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = null!; // Shipment, Order, Invoice...

        [Required]
        public Guid EntityId { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        public DateTime ActionAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(4000)]
        public string? OldValuesJson { get; set; }

        [MaxLength(4000)]
        public string? NewValuesJson { get; set; }

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}