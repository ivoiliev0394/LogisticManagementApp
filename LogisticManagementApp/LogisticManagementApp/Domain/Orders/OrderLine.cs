using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Orders
{
    public class OrderLine : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int LineNo { get; set; } = 1;

        [Required]
        [MaxLength(300)]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal? Quantity { get; set; }

        [MaxLength(30)]
        public string? QuantityUnit { get; set; } // pcs, boxes, pallets, kg...

        /// <summary>
        /// Тегло/обем на линията (по избор). Може да се ползва за предварителна калкулация.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? GrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        /// <summary>
        /// Ако е опасен товар (по-късно ще вържем DG декларации).
        /// </summary>
        [Required]
        public bool IsDangerousGoods { get; set; } = false;

        [MaxLength(20)]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        public string? OriginCountry { get; set; }
    }
}
