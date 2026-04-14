using LogisticManagementApp.Domain.Common;
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
        public string EventKey { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Channel { get; set; } = null!; // Email, InApp, SMS

        public bool IsEnabled { get; set; } = true;
    }
}