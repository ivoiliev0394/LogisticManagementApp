using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Pricing;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Pricing
{
    public class ServiceLevel : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = null!; // STD, EXP, SDD

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public ServiceLevelType ServiceLevelType { get; set; } = ServiceLevelType.Standard;

        [Required]
        public TransportMode TransportMode { get; set; } = TransportMode.Road;

        [Range(0, double.MaxValue)]
        public decimal? MaxWeightKg { get; set; }

        [Range(0, int.MaxValue)]
        public int? EstimatedTransitDays { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
    }
}
