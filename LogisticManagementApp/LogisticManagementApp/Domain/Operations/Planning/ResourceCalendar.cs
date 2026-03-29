using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Operations.Planning
{
    public class ResourceCalendar : BaseEntity
    {
        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public DateTime DateUtc { get; set; }

        [Required]
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;

        [Range(0, double.MaxValue)]
        public decimal? PlannedCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ReservedCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UsedCapacity { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
