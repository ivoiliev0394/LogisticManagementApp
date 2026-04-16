using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.TrackingEvents
{
    public class CompanyTrackingEventCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event type")]
        public TrackingEventType EventType { get; set; }

        [Required]
        [Display(Name = "Event time UTC")]
        public DateTime EventTimeUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "Location")]
        public Guid? LocationId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Details")]
        public string? Details { get; set; }

        [MaxLength(50)]
        [Display(Name = "Source")]
        public string? Source { get; set; }

        public IEnumerable<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();
    }

}
