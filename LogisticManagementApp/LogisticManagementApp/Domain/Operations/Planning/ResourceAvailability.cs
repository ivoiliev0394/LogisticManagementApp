using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Operations.Planning
{
    public class ResourceAvailability : BaseEntity
    {
        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public DateTime AvailableFromUtc { get; set; }

        [Required]
        public DateTime AvailableToUtc { get; set; }

        [Required]
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;

        [MaxLength(200)]
        public string? Reason { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
