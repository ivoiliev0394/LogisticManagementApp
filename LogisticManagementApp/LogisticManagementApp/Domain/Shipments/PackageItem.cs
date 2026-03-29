using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class PackageItem : BaseEntity
    {
        [Required]
        public Guid PackageId { get; set; }

        [ForeignKey(nameof(PackageId))]
        public Package Package { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string Description { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; } = 1;

        [MaxLength(30)]
        public string? Unit { get; set; } // pcs, kg, box...

        [MaxLength(20)]
        public string? HsCode { get; set; }

        [MaxLength(100)]
        public string? OriginCountry { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitPrice { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; }
    }
}
