using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Sea
{
    public class Vessel : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(20)]
        public string? ImoNumber { get; set; }

        [MaxLength(20)]
        public string? MmsiNumber { get; set; }

        [Required]
        public VesselType VesselType { get; set; } = VesselType.Other;

        [Range(0, int.MaxValue)]
        public int? CapacityTeu { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DeadweightTons { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<Voyage> Voyages { get; set; } = new List<Voyage>();
    }
}
