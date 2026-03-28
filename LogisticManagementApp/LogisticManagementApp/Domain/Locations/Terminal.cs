
using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Locations
{
    public class Terminal : BaseEntity
    {
        [Required]
        public Guid LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;

        [Required]
        public TerminalType TerminalType { get; set; } = TerminalType.Other;

        [MaxLength(50)]
        public string? TerminalCode { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CapacityCbm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CapacityTons { get; set; }

        [Required]
        public bool IsBonded { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? OperatingHours { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
