using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Locations
{
    public class Warehouse : BaseEntity
    {
        /// <summary>
        /// 1:1 към Location (warehouse е тип локация с доп. свойства)
        /// </summary>
        [Required]
        public Guid LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;

        [Required]
        public WarehouseType WarehouseType { get; set; } = WarehouseType.General;

        /// <summary>
        /// Капацитет по избор (за статистики/заетост).
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? CapacityCubicMeters { get; set; }

        /// <summary>
        /// “Cut-off” за прием на товар (напр. 18:00)
        /// </summary>
        public TimeSpan? CutOffTime { get; set; }

        [Required]
        public bool IsBonded { get; set; } = false;

        [MaxLength(200)]
        public string? OperatingHours { get; set; } // "Mon-Fri 09:00-18:00"

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
