using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Security
{
    public class UserSession : BaseEntity
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string SessionToken { get; set; } = null!;

        [Required]
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAtUtc { get; set; }

        public DateTime? LastSeenAtUtc { get; set; }

        [Required]
        public UserSessionStatus Status { get; set; } = UserSessionStatus.Active;

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(300)]
        public string? UserAgent { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}