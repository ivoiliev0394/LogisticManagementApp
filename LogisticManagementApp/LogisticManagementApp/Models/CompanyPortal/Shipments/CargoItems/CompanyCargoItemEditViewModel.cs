using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.CargoItems
{
    public class CompanyCargoItemEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cargo Item Type")]
        public CargoItemType CargoItemType { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }

        [MaxLength(30)]
        [Display(Name = "Unit Of Measure")]
        public string? UnitOfMeasure { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Gross Weight (kg)")]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Net Weight (kg)")]
        public decimal? NetWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Volume (cbm)")]
        public decimal? VolumeCbm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length (cm)")]
        public decimal? LengthCm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width (cm)")]
        public decimal? WidthCm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Height (cm)")]
        public decimal? HeightCm { get; set; }

        [MaxLength(20)]
        [Display(Name = "HS Code")]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "Origin Country")]
        public string? OriginCountry { get; set; }

        [Display(Name = "Is Stackable")]
        public bool IsStackable { get; set; }

        [Display(Name = "Is Fragile")]
        public bool IsFragile { get; set; }

        [MaxLength(300)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
