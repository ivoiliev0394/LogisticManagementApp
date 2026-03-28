using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Routes
{
    public class RouteStop : BaseEntity
    {
        [Required]
        public Guid RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public Route Route { get; set; } = null!;

        [Required]
        public Guid LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int SequenceNo { get; set; } = 1;

        [Required]
        public RouteStopType StopType { get; set; } = RouteStopType.Other;

        public TimeSpan? PlannedArrivalTime { get; set; }

        public TimeSpan? PlannedDepartureTime { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
