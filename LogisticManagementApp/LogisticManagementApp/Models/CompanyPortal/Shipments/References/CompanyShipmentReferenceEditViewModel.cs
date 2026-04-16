using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.References
{
    public class CompanyShipmentReferenceEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Reference Type")]
        public ShipmentReferenceType ReferenceType { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Reference Value")]
        public string ReferenceValue { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
