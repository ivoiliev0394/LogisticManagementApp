using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Packages
{
    public class CompanyPackageCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [Display(Name = "Package number")]
        public string PackageNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Package type")]
        public PackageType PackageType { get; set; } = PackageType.Box;

        [Range(0, double.MaxValue)]
        [Display(Name = "Weight (kg)")]
        public decimal WeightKg { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length (cm)")]
        public decimal? LengthCm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width (cm)")]
        public decimal? WidthCm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Height (cm)")]
        public decimal? HeightCm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Volume (cbm)")]
        public decimal? VolumeCbm { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
