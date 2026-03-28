using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Routes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Routes
{
    public class RoutePlan : BaseEntity
    {
        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;

        [Required]
        public DateTime PlanDateUtc { get; set; }

        [Required]
        public RoutePlanStatus Status { get; set; } = RoutePlanStatus.Draft;

        [MaxLength(100)]
        public string? PlanReference { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<RoutePlanStop> Stops { get; set; } = new List<RoutePlanStop>();
        public ICollection<RoutePlanShipment> Shipments { get; set; } = new List<RoutePlanShipment>();
    }
}
