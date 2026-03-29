using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Rail
{
    public class Train : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string TrainNumber { get; set; } = null!;

        [Required]
        public TrainType TrainType { get; set; } = TrainType.Freight;

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

        public ICollection<RailMovement> RailMovements { get; set; } = new List<RailMovement>();
    }
}
