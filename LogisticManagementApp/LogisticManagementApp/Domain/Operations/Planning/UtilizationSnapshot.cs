using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Operations.Planning
{
    public class UtilizationSnapshot : BaseEntity
    {
        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public DateTime SnapshotDateUtc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TotalCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UsedCapacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? FreeCapacity { get; set; }

        [Range(0, 100)]
        public decimal? UtilizationPercent { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
