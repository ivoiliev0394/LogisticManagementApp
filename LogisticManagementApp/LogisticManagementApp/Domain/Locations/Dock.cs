using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Locations
{
    public class Dock : BaseEntity
    {
        /// <summary>
        /// Dock може да е към warehouse или към generic location/terminal.
        /// </summary>
        public Guid? WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; set; }

        public Guid? LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location? Location { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        public DockType DockType { get; set; } = DockType.LoadingDock;

        [Required]
        public DockStatus Status { get; set; } = DockStatus.Available;

        [Range(0, double.MaxValue)]
        public decimal? MaxWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxVolumeCbm { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
