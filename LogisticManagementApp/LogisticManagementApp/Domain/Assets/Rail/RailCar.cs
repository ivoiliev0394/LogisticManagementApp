using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Rail
{
    public class RailCar : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string RailCarNumber { get; set; } = null!;

        [Required]
        public RailCarType RailCarType { get; set; } = RailCarType.Boxcar;

        [Range(0, double.MaxValue)]
        public decimal? MaxWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxVolumeCbm { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
