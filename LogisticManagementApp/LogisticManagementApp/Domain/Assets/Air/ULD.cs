using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Air
{
    public class ULD : BaseEntity
    {
        public Guid? OwnerCompanyId { get; set; }

        [ForeignKey(nameof(OwnerCompanyId))]
        public Company? OwnerCompany { get; set; }

        [Required]
        [MaxLength(30)]
        public string UldNumber { get; set; } = null!;

        [Required]
        public UldType UldType { get; set; } = UldType.Other;

        [Required]
        public UldStatus Status { get; set; } = UldStatus.Available;

        [Range(0, double.MaxValue)]
        public decimal? TareWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxGrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<ShipmentULD> ShipmentULDs { get; set; } = new List<ShipmentULD>();
    }
}
