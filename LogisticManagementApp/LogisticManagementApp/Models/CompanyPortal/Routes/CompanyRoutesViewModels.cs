using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Routes
{
    public class RouteCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyRoutesHomeViewModel
    {
        public IEnumerable<RouteCardViewModel> Cards { get; set; }
            = Enumerable.Empty<RouteCardViewModel>();
    }

    public class RouteListItemViewModel
    {
        public Guid Id { get; set; }
        public string RouteCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TransportMode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class RouteCreateViewModel
    {
        [Required, StringLength(50)]
        public string RouteCode { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public TransportMode TransportMode { get; set; } = TransportMode.Road;

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class RouteEditViewModel : RouteCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RouteStopListItemViewModel
    {
        public Guid Id { get; set; }
        public string RouteDisplay { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public string StopType { get; set; } = string.Empty;

        public TimeSpan? PlannedArrivalTime { get; set; }
        public TimeSpan? PlannedDepartureTime { get; set; }

        public string? Notes { get; set; }
    }

    public class RouteStopCreateViewModel
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Range(1, int.MaxValue)]
        public int SequenceNo { get; set; } = 1;

        [Required]
        public RouteStopType StopType { get; set; } = RouteStopType.Other;

        public TimeSpan? PlannedArrivalTime { get; set; }
        public TimeSpan? PlannedDepartureTime { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> RouteOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> LocationOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RouteStopEditViewModel : RouteStopCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RoutePlanListItemViewModel
    {
        public Guid Id { get; set; }
        public string RouteDisplay { get; set; } = string.Empty;
        public DateTime PlanDateUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PlanReference { get; set; }
        public string? Notes { get; set; }
    }

    public class RoutePlanCreateViewModel
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        public DateTime PlanDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public RoutePlanStatus Status { get; set; } = RoutePlanStatus.Draft;

        [StringLength(100)]
        public string? PlanReference { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> RouteOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RoutePlanEditViewModel : RoutePlanCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RoutePlanStopListItemViewModel
    {
        public Guid Id { get; set; }
        public string RoutePlanDisplay { get; set; } = string.Empty;
        public string? RouteStopDisplay { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public string StopType { get; set; } = string.Empty;

        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }

        public string? Notes { get; set; }
    }

    public class RoutePlanStopCreateViewModel
    {
        [Required]
        public Guid RoutePlanId { get; set; }

        public Guid? RouteStopId { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int SequenceNo { get; set; } = 1;

        [Required]
        public RouteStopType StopType { get; set; } = RouteStopType.Other;

        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> RoutePlanOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> RouteStopOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> LocationOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RoutePlanStopEditViewModel : RoutePlanStopCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RoutePlanShipmentListItemViewModel
    {
        public Guid Id { get; set; }
        public string RoutePlanDisplay { get; set; } = string.Empty;
        public string ShipmentNo { get; set; } = string.Empty;
        public string? PickupStopDisplay { get; set; }
        public string? DeliveryStopDisplay { get; set; }
        public int Priority { get; set; }
        public string? Notes { get; set; }
    }

    public class RoutePlanShipmentCreateViewModel
    {
        [Required]
        public Guid RoutePlanId { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public Guid? PickupStopId { get; set; }
        public Guid? DeliveryStopId { get; set; }

        [Range(1, int.MaxValue)]
        public int Priority { get; set; } = 1;

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> RoutePlanOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> ShipmentOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> RoutePlanStopOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RoutePlanShipmentEditViewModel : RoutePlanShipmentCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}