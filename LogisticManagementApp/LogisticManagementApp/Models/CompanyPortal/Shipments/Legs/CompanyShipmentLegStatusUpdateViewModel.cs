using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Legs
{
    public class CompanyShipmentLegStatusUpdateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [Required]
        public Guid ShipmentLegId { get; set; }

        [Required]
        public LegStatus NewStatus { get; set; }

        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
