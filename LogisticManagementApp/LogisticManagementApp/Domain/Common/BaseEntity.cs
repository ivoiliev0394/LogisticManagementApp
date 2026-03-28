using LogisticManagementApp.Domain.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Common
{
    public class BaseEntity : IAuditable, ISoftDeletable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Audit
        [Required]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        // Soft delete
        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }
    }
}
