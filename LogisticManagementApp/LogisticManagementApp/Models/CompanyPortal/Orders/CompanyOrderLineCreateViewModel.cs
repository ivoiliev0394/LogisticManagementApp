using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Orders
{
    public class CompanyOrderLineCreateViewModel
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Ред")]
        public int LineNo { get; set; } = 1;

        [Required]
        [MaxLength(300)]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        [Display(Name = "Количество")]
        public decimal? Quantity { get; set; }

        [MaxLength(30)]
        [Display(Name = "Единица")]
        public string? QuantityUnit { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Тегло (kg)")]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Обем (cbm)")]
        public decimal? VolumeCbm { get; set; }

        [Display(Name = "Опасен товар")]
        public bool IsDangerousGoods { get; set; }

        [MaxLength(20)]
        [Display(Name = "HS Code")]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        [Display(Name = "Страна на произход")]
        public string? OriginCountry { get; set; }
    }
}
