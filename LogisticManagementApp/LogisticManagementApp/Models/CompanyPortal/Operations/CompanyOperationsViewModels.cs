using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Operations
{
    public class OperationCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyOperationsHomeViewModel
    {
        public IEnumerable<OperationCardViewModel> Cards { get; set; } = Enumerable.Empty<OperationCardViewModel>();
    }

    public class NotificationListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string? RecipientUserDisplay { get; set; }
        public bool IsRead { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public string? RelatedEntityType { get; set; }
        public string? Notes { get; set; }
    }

    public class NotificationCreateViewModel
    {
        [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
        [Required, StringLength(1000)] public string Message { get; set; } = string.Empty;
        [Required] public NotificationType NotificationType { get; set; } = NotificationType.Info;
        [Required] public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        [StringLength(450)] public string? RecipientUserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }
        [StringLength(100)] public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> UserOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class NotificationEditViewModel : NotificationCreateViewModel { [Required] public Guid Id { get; set; } }

    public class NotificationSubscriptionListItemViewModel
    {
        public Guid Id { get; set; }
        public string? UserDisplay { get; set; }
        public string EventKey { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? Notes { get; set; }
    }

    public class NotificationSubscriptionCreateViewModel
    {
        [StringLength(450)] public string? UserId { get; set; }
        [Required, StringLength(100)] public string EventKey { get; set; } = string.Empty;
        [Required] public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        public bool IsEnabled { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> UserOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class NotificationSubscriptionEditViewModel : NotificationSubscriptionCreateViewModel { [Required] public Guid Id { get; set; } }

    public class AuditLogListItemViewModel
    {
        public Guid Id { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? UserName { get; set; }
        public DateTime ActionAtUtc { get; set; }
        public string? IpAddress { get; set; }
        public string? Notes { get; set; }
    }

    public class SavedFilterListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? UserDisplay { get; set; }
        public bool IsDefault { get; set; }
        public string FilterJson { get; set; } = string.Empty;
    }

    public class SavedFilterCreateViewModel
    {
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(100)] public string EntityType { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string FilterJson { get; set; } = string.Empty;
        [StringLength(450)] public string? UserId { get; set; }
        public bool IsDefault { get; set; }
        public IEnumerable<SelectListItem> UserOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class SavedFilterEditViewModel : SavedFilterCreateViewModel { [Required] public Guid Id { get; set; } }

    public class CompanyDashboardConfigListItemViewModel
    {
        public Guid Id { get; set; }
        public string DashboardKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string LayoutJson { get; set; } = string.Empty;
        public string? WidgetSettingsJson { get; set; }
    }

    public class CompanyDashboardConfigCreateViewModel
    {
        [Required, StringLength(100)] public string DashboardKey { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string LayoutJson { get; set; } = string.Empty;
        [StringLength(4000)] public string? WidgetSettingsJson { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class CompanyDashboardConfigEditViewModel : CompanyDashboardConfigCreateViewModel { [Required] public Guid Id { get; set; } }

    public class BookingListItemViewModel
    {
        public Guid Id { get; set; }
        public string BookingNo { get; set; } = string.Empty;
        public string? CarrierCompanyName { get; set; }
        public string? ShipmentNo { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransportMode { get; set; } = string.Empty;
        public DateTime? RequestedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public string? CarrierReference { get; set; }
        public string? Notes { get; set; }
    }

    public class BookingCreateViewModel
    {
        [Required, StringLength(50)] public string BookingNo { get; set; } = string.Empty;
        public Guid? CarrierCompanyId { get; set; }
        public Guid? ShipmentId { get; set; }
        [Required] public BookingStatus Status { get; set; } = BookingStatus.Draft;
        [Required] public TransportMode TransportMode { get; set; } = TransportMode.Road;
        public DateTime? RequestedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        [StringLength(100)] public string? CarrierReference { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> CarrierCompanyOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class BookingEditViewModel : BookingCreateViewModel { [Required] public Guid Id { get; set; } }

    public class BookingLegListItemViewModel
    {
        public Guid Id { get; set; }
        public string BookingDisplay { get; set; } = string.Empty;
        public int LegNo { get; set; }
        public string? ShipmentLegDisplay { get; set; }
        public string? OriginLocationName { get; set; }
        public string? DestinationLocationName { get; set; }
        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }
        public string? CarrierReference { get; set; }
        public string? Notes { get; set; }
    }

    public class BookingLegCreateViewModel
    {
        [Required] public Guid BookingId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        [Range(1, int.MaxValue)] public int LegNo { get; set; } = 1;
        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }
        [StringLength(100)] public string? CarrierReference { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> BookingOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class BookingLegEditViewModel : BookingLegCreateViewModel { [Required] public Guid Id { get; set; } }

    public class ConsolidationListItemViewModel
    {
        public Guid Id { get; set; }
        public string ConsolidationNo { get; set; } = string.Empty;
        public string ConsolidationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransportMode { get; set; } = string.Empty;
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public string? MasterReference { get; set; }
        public string? Notes { get; set; }
    }

    public class ConsolidationShipmentListItemViewModel
    {
        public Guid Id { get; set; }
        public string ConsolidationDisplay { get; set; } = string.Empty;
        public string ShipmentNo { get; set; } = string.Empty;
        public string? ShipmentLegDisplay { get; set; }
        public decimal? AllocatedWeightKg { get; set; }
        public decimal? AllocatedVolumeCbm { get; set; }
        public string? Notes { get; set; }
    }

    public class ConsolidationShipmentCreateViewModel
    {
        [Required] public Guid ConsolidationId { get; set; }
        [Required] public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        [Range(0, double.MaxValue)] public decimal? AllocatedWeightKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? AllocatedVolumeCbm { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ConsolidationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class ConsolidationShipmentEditViewModel : ConsolidationShipmentCreateViewModel { [Required] public Guid Id { get; set; } }

    public class ResourceCalendarListItemViewModel
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceDisplay { get; set; } = string.Empty;
        public DateTime DateUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? PlannedCapacity { get; set; }
        public decimal? ReservedCapacity { get; set; }
        public decimal? UsedCapacity { get; set; }
        public string? Notes { get; set; }
    }

    public class ResourceCalendarCreateViewModel
    {
        [Required] public ResourceType ResourceType { get; set; } = ResourceType.Vehicle;
        [Required] public Guid ResourceId { get; set; }
        [Required] public DateTime DateUtc { get; set; } = DateTime.UtcNow.Date;
        [Required] public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
        [Range(0, double.MaxValue)] public decimal? PlannedCapacity { get; set; }
        [Range(0, double.MaxValue)] public decimal? ReservedCapacity { get; set; }
        [Range(0, double.MaxValue)] public decimal? UsedCapacity { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class ResourceCalendarEditViewModel : ResourceCalendarCreateViewModel { [Required] public Guid Id { get; set; } }

    public class ResourceAvailabilityListItemViewModel
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceDisplay { get; set; } = string.Empty;
        public DateTime AvailableFromUtc { get; set; }
        public DateTime AvailableToUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class ResourceAvailabilityCreateViewModel
    {
        [Required] public ResourceType ResourceType { get; set; } = ResourceType.Vehicle;
        [Required] public Guid ResourceId { get; set; }
        [Required] public DateTime AvailableFromUtc { get; set; } = DateTime.UtcNow;
        [Required] public DateTime AvailableToUtc { get; set; } = DateTime.UtcNow.AddHours(1);
        [Required] public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;
        [StringLength(200)] public string? Reason { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class ResourceAvailabilityEditViewModel : ResourceAvailabilityCreateViewModel { [Required] public Guid Id { get; set; } }

    public class CapacityReservationListItemViewModel
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceDisplay { get; set; } = string.Empty;
        public string? ShipmentNo { get; set; }
        public string? ShipmentLegDisplay { get; set; }
        public decimal? ReservedWeightKg { get; set; }
        public decimal? ReservedVolumeCbm { get; set; }
        public int? ReservedUnitCount { get; set; }
        public DateTime? ReservedFromUtc { get; set; }
        public DateTime? ReservedToUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class CapacityReservationCreateViewModel
    {
        [Required] public ResourceType ResourceType { get; set; } = ResourceType.Vehicle;
        [Required] public Guid ResourceId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        [Range(0, double.MaxValue)] public decimal? ReservedWeightKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? ReservedVolumeCbm { get; set; }
        [Range(0, int.MaxValue)] public int? ReservedUnitCount { get; set; }
        public DateTime? ReservedFromUtc { get; set; }
        public DateTime? ReservedToUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class CapacityReservationEditViewModel : CapacityReservationCreateViewModel { [Required] public Guid Id { get; set; } }

    public class AssignmentListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentLegDisplay { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceDisplay { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? AssignedAtUtc { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Notes { get; set; }
    }

    public class AssignmentCreateViewModel
    {
        [Required] public Guid ShipmentLegId { get; set; }
        [Required] public ResourceType ResourceType { get; set; } = ResourceType.Vehicle;
        [Required] public Guid ResourceId { get; set; }
        [Required] public AssignmentStatus Status { get; set; } = AssignmentStatus.Planned;
        public DateTime? AssignedAtUtc { get; set; }
        [StringLength(100)] public string? ReferenceNo { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class AssignmentEditViewModel : AssignmentCreateViewModel { [Required] public Guid Id { get; set; } }

    public class UtilizationSnapshotListItemViewModel
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceDisplay { get; set; } = string.Empty;
        public DateTime SnapshotDateUtc { get; set; }
        public decimal? TotalCapacity { get; set; }
        public decimal? UsedCapacity { get; set; }
        public decimal? FreeCapacity { get; set; }
        public decimal? UtilizationPercent { get; set; }
        public string? Notes { get; set; }
    }
}
