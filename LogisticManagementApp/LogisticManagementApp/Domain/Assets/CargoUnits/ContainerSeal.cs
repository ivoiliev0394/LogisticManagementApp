using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.CargoUnits
{
    public class ContainerSeal : BaseEntity
    {
        [Required]
        public Guid ContainerId { get; set; }

        [ForeignKey(nameof(ContainerId))]
        public Container Container { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string SealNumber { get; set; } = null!;

        public DateTime? AppliedAtUtc { get; set; }

        [MaxLength(150)]
        public string? AppliedBy { get; set; }

        public DateTime? RemovedAtUtc { get; set; }

        [MaxLength(150)]
        public string? RemovedBy { get; set; }

        [Required]
        public bool IsActiveSeal { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
