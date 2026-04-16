using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Tags
{
    public class CompanyShipmentTagCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tag Type")]
        public ShipmentTagType TagType { get; set; } = ShipmentTagType.Other;

        [MaxLength(100)]
        [Display(Name = "Custom Value")]
        public string? CustomValue { get; set; }

        [MaxLength(200)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
