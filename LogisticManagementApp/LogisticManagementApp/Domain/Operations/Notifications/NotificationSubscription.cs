using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Notifications
{
    public class NotificationSubscription : BaseEntity
    {
        [MaxLength(450)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public Guid? CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventKey { get; set; } = null!; // Shipment.Delivered, Invoice.Overdue

        [Required]
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

        [Required]
        public bool IsEnabled { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}