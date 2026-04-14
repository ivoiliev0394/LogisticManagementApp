using LogisticManagementApp.Domain.Common;
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
        [MaxLength(100)]
        public string NotificationType { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(2000)]
        public string? Message { get; set; }

        public bool IsRead { get; set; }

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; }

        public Guid? RelatedEntityId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}