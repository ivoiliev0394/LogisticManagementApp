using LogisticManagementApp.Models.CompanyPortal.Shipments.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Containers
{
    public class CompanyShipmentContainerCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Container")]
        public Guid ContainerId { get; set; }

        [Display(Name = "Shipment Leg")]
        public Guid? ShipmentLegId { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Gross Weight (kg)")]
        public decimal? GrossWeightKg { get; set; }

        [MaxLength(50)]
        [Display(Name = "Seal Number")]
        public string? SealNumber { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IList<CompanyContainerOptionViewModel> ContainerOptions { get; set; }
            = new List<CompanyContainerOptionViewModel>();

        public IList<CompanyShipmentLegOptionViewModel> ShipmentLegOptions { get; set; }
            = new List<CompanyShipmentLegOptionViewModel>();
    }
}
