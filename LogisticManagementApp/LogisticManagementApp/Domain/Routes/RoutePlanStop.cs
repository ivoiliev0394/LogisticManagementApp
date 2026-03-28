using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Routes
{
    public class RoutePlanStop : BaseEntity
    {
        [Required]
        public Guid RoutePlanId { get; set; }

        [ForeignKey(nameof(RoutePlanId))]
        public RoutePlan RoutePlan { get; set; } = null!;

        public Guid? RouteStopId { get; set; }

        [ForeignKey(nameof(RouteStopId))]
        public RouteStop? RouteStop { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int SequenceNo { get; set; } = 1;

        [Required]
        public RouteStopType StopType { get; set; } = RouteStopType.Other;

        public DateTime? PlannedArrivalUtc { get; set; }

        public DateTime? PlannedDepartureUtc { get; set; }

        public DateTime? ActualArrivalUtc { get; set; }

        public DateTime? ActualDepartureUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
