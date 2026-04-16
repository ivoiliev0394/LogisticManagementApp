using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Voyages
{
    public class CompanyShipmentVoyageCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Voyage Id")]
        public Guid VoyageId { get; set; }

        [Display(Name = "Shipment Leg Id")]
        public Guid? ShipmentLegId { get; set; }

        [MaxLength(100)]
        [Display(Name = "Booking Reference")]
        public string? BookingReference { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IList<SelectListItem> ShipmentLegOptions { get; set; } = new List<SelectListItem>();
    }
}
