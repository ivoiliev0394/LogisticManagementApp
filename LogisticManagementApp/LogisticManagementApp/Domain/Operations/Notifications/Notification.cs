using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Notifications
{
    public class Notification : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = null!;

        [Required]
        public NotificationType NotificationType { get; set; } = NotificationType.Info;

        [Required]
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

        /// <summary>
        /// Оставяме ги като Guid, без FK към User засега, защото Identity е за по-късно.
        /// </summary>
        [MaxLength(450)]
        public string? RecipientUserId { get; set; }

        [ForeignKey(nameof(RecipientUserId))]
        public AspNetUsers? RecipientUser { get; set; }

        public Guid? RecipientCompanyId { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        public DateTime? ReadAtUtc { get; set; }

        public DateTime? SentAtUtc { get; set; }

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; } // Shipment, Invoice, Trip...

        public Guid? RelatedEntityId { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
