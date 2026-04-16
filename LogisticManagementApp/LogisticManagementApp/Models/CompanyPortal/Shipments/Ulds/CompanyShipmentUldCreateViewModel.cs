using LogisticManagementApp.Models.CompanyPortal.Shipments.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Ulds
{
    public class CompanyShipmentUldCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ULD")]
        public Guid UldId { get; set; }

        [Display(Name = "Shipment Leg")]
        public Guid? ShipmentLegId { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Gross Weight (kg)")]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Volume (cbm)")]
        public decimal? VolumeCbm { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IList<CompanyUldOptionViewModel> UldOptions { get; set; }
            = new List<CompanyUldOptionViewModel>();

        public IList<CompanyShipmentLegOptionViewModel> ShipmentLegOptions { get; set; }
            = new List<CompanyShipmentLegOptionViewModel>();
    }
}
