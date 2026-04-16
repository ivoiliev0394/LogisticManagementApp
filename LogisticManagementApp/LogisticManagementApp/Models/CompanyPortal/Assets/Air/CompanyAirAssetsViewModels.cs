using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Assets.Air
{
    public class AirAssetCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyAirHomeViewModel
    {
        public IEnumerable<AirAssetCardViewModel> Cards { get; set; } = Enumerable.Empty<AirAssetCardViewModel>();
    }

    public class AircraftListItemViewModel
    {
        public Guid Id { get; set; }
        public string TailNumber { get; set; } = string.Empty;
        public string AircraftType { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public decimal? MaxPayloadKg { get; set; }
        public decimal? MaxVolumeCbm { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class AircraftCreateViewModel
    {
        [Required, StringLength(20)] public string TailNumber { get; set; } = string.Empty;
        [Required] public AircraftType AircraftType { get; set; } = AircraftType.Freighter;
        [StringLength(100)] public string? Manufacturer { get; set; }
        [StringLength(100)] public string? Model { get; set; }
        [Range(0, double.MaxValue)] public decimal? MaxPayloadKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? MaxVolumeCbm { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class AircraftEditViewModel : AircraftCreateViewModel { [Required] public Guid Id { get; set; } }

    public class FlightListItemViewModel
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string AircraftDisplay { get; set; } = string.Empty;
        public string? OriginLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class FlightCreateViewModel
    {
        [Required] public Guid AircraftId { get; set; }
        [Required, StringLength(20)] public string FlightNumber { get; set; } = string.Empty;
        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        [Required] public FlightStatus Status { get; set; } = FlightStatus.Planned;
        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> AircraftOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class FlightEditViewModel : FlightCreateViewModel { [Required] public Guid Id { get; set; } }

    public class FlightSegmentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string AircraftDisplay { get; set; } = string.Empty;
        public int SegmentNo { get; set; }
        public string? OriginLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class FlightSegmentCreateViewModel
    {
        [Required] public Guid FlightId { get; set; }
        [Range(1, int.MaxValue)] public int SegmentNo { get; set; } = 1;
        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> FlightOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class FlightSegmentEditViewModel : FlightSegmentCreateViewModel { [Required] public Guid Id { get; set; } }

    public class AirCrewMemberListItemViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string CrewRole { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class AirCrewMemberCreateViewModel
    {
        [Required, StringLength(150)] public string FullName { get; set; } = string.Empty;
        [Required] public AirCrewRole CrewRole { get; set; } = AirCrewRole.Other;
        [StringLength(50)] public string? LicenseNumber { get; set; }
        [StringLength(30)] public string? Phone { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class AirCrewMemberEditViewModel : AirCrewMemberCreateViewModel { [Required] public Guid Id { get; set; } }

    public class AirCrewAssignmentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public Guid AirCrewMemberId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string AircraftDisplay { get; set; } = string.Empty;
        public string CrewMemberName { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
        public DateTime? AssignedAtUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class AirCrewAssignmentCreateViewModel
    {
        [Required] public Guid FlightId { get; set; }
        [Required] public Guid AirCrewMemberId { get; set; }
        [Required] public AirCrewRole AssignedRole { get; set; } = AirCrewRole.Other;
        public DateTime? AssignedAtUtc { get; set; } = DateTime.UtcNow;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> FlightOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> AirCrewMemberOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class AirCrewAssignmentEditViewModel : AirCrewAssignmentCreateViewModel { [Required] public Guid Id { get; set; } }

    public class UldListItemViewModel
    {
        public Guid Id { get; set; }
        public string UldNumber { get; set; } = string.Empty;
        public string UldType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? TareWeightKg { get; set; }
        public decimal? MaxGrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class UldCreateViewModel
    {
        [Required, StringLength(30)] public string UldNumber { get; set; } = string.Empty;
        [Required] public UldType UldType { get; set; } = UldType.Other;
        [Required] public UldStatus Status { get; set; } = UldStatus.Available;
        [Range(0, double.MaxValue)] public decimal? TareWeightKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? MaxGrossWeightKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? VolumeCbm { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class UldEditViewModel : UldCreateViewModel { [Required] public Guid Id { get; set; } }
}
