using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.PackageItems
{
    public class CompanyPackageItemCreateViewModel
    {
        [Required]
        public Guid PackageId { get; set; }

        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;
        public string PackageNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; } = 1;

        [MaxLength(30)]
        [Display(Name = "Unit")]
        public string? Unit { get; set; }

        [MaxLength(20)]
        [Display(Name = "HS Code")]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "Origin Country")]
        public string? OriginCountry { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }

        [MaxLength(3)]
        [Display(Name = "Currency")]
        public string? Currency { get; set; }
    }
}
