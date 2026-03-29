using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.CargoUnits
{
    public class Container : BaseEntity
    {
        public Guid? OwnerCompanyId { get; set; }

        [ForeignKey(nameof(OwnerCompanyId))]
        public Company? OwnerCompany { get; set; }

        [Required]
        [MaxLength(20)]
        public string ContainerNumber { get; set; } = null!;

        [Required]
        public ContainerType ContainerType { get; set; } = ContainerType.GP20;

        [Required]
        public ContainerStatus Status { get; set; } = ContainerStatus.Available;

        [Range(0, double.MaxValue)]
        public decimal? TareWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxGrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [MaxLength(50)]
        public string? SealNumber { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<ShipmentContainer> ShipmentContainers { get; set; } = new List<ShipmentContainer>();
    }
}
