using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Legs
{
    public class CompanyShipmentLegEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        [Display(Name = "Leg No")]
        public int LegNo { get; set; }

        [Required]
        [Display(Name = "Mode")]
        public TransportMode Mode { get; set; }

        [Required]
        [Display(Name = "Origin location")]
        public Guid OriginLocationId { get; set; }

        [Required]
        [Display(Name = "Destination location")]
        public Guid DestinationLocationId { get; set; }

        [Display(Name = "Current status")]
        public string CurrentStatus { get; set; } = string.Empty;

        [Display(Name = "ETD UTC")]
        public DateTime? ETD_Utc { get; set; }

        [Display(Name = "ETA UTC")]
        public DateTime? ETA_Utc { get; set; }

        [Display(Name = "ATD UTC")]
        public DateTime? ATD_Utc { get; set; }

        [Display(Name = "ATA UTC")]
        public DateTime? ATA_Utc { get; set; }

        [MaxLength(100)]
        [Display(Name = "Carrier reference")]
        public string? CarrierReference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();
    }
}
