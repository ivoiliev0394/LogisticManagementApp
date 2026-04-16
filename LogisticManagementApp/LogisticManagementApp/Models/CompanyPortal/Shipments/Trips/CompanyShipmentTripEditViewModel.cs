using LogisticManagementApp.Models.CompanyPortal.Shipments.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Trips
{
    public class CompanyShipmentTripEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Trip")]
        public Guid TripId { get; set; }

        [Display(Name = "Shipment Leg")]
        public Guid? ShipmentLegId { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IList<CompanyShipmentTripOptionViewModel> TripOptions { get; set; }
            = new List<CompanyShipmentTripOptionViewModel>();

        public IList<CompanyShipmentLegOptionViewModel> ShipmentLegOptions { get; set; }
            = new List<CompanyShipmentLegOptionViewModel>();
    }
}
