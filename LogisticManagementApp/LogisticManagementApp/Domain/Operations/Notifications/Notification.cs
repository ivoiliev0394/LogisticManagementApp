using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Notifications
{
    public class Notification : BaseEntity
    {
        [MaxLength(450)]
        public string? RecipientUserId { get; set; }

        [ForeignKey(nameof(RecipientUserId))]
        public ApplicationUser? RecipientUser { get; set; }

        public Guid? RecipientCompanyId { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; } = NotificationType.Info;

        [Required]
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(2000)]
        public string? Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAtUtc { get; set; }

        public DateTime? SentAtUtc { get; set; }

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; }

        public Guid? RelatedEntityId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}