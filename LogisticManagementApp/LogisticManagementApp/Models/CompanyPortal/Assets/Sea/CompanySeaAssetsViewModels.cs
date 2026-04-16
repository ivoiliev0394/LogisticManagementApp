using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Assets.Sea
{
    public class AssetsHomeCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string ButtonText { get; set; } = "Отвори";
    }

    public class CompanyAssetsHomeViewModel
    {
        public IEnumerable<AssetsHomeCardViewModel> Cards { get; set; } = Enumerable.Empty<AssetsHomeCardViewModel>();
    }

    public class SeaAssetCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanySeaHomeViewModel
    {
        public IEnumerable<SeaAssetCardViewModel> Cards { get; set; } = Enumerable.Empty<SeaAssetCardViewModel>();
    }

    public class VesselListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImoNumber { get; set; }
        public string? MmsiNumber { get; set; }
        public string VesselType { get; set; } = string.Empty;
        public int? CapacityTeu { get; set; }
        public decimal? DeadweightTons { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class VesselCreateViewModel
    {
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [StringLength(20)] public string? ImoNumber { get; set; }
        [StringLength(20)] public string? MmsiNumber { get; set; }
        [Required] public VesselType VesselType { get; set; } = VesselType.Other;
        [Range(0, int.MaxValue)] public int? CapacityTeu { get; set; }
        [Range(0, double.MaxValue)] public decimal? DeadweightTons { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class VesselEditViewModel : VesselCreateViewModel { [Required] public Guid Id { get; set; } }

    public class VesselPositionListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VesselId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public DateTime PositionTimeUtc { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal? SpeedKnots { get; set; }
        public decimal? CourseDegrees { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
    }

    public class VesselPositionCreateViewModel
    {
        [Required] public Guid VesselId { get; set; }
        [Required] public DateTime PositionTimeUtc { get; set; } = DateTime.UtcNow;
        [Range(-90, 90)] public decimal Latitude { get; set; }
        [Range(-180, 180)] public decimal Longitude { get; set; }
        [Range(0, double.MaxValue)] public decimal? SpeedKnots { get; set; }
        [Range(0, 360)] public decimal? CourseDegrees { get; set; }
        [StringLength(200)] public string? Source { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> VesselOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class VesselPositionEditViewModel : VesselPositionCreateViewModel { [Required] public Guid Id { get; set; } }

    public class VoyageListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VesselId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string VoyageNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public string OriginPort { get; set; } = string.Empty;
        public string DestinationPort { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class VoyageCreateViewModel
    {
        [Required] public Guid VesselId { get; set; }
        [Required, StringLength(50)] public string VoyageNumber { get; set; } = string.Empty;
        [Required] public TripStatus Status { get; set; } = TripStatus.Planned;
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        [Required, StringLength(100)] public string OriginPort { get; set; } = string.Empty;
        [Required, StringLength(100)] public string DestinationPort { get; set; } = string.Empty;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> VesselOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class VoyageEditViewModel : VoyageCreateViewModel { [Required] public Guid Id { get; set; } }

    public class VoyageStopListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VoyageId { get; set; }
        public string VoyageNumber { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class VoyageStopCreateViewModel
    {
        [Required] public Guid VoyageId { get; set; }
        [Required] public Guid LocationId { get; set; }
        [Range(1, int.MaxValue)] public int SequenceNumber { get; set; } = 1;
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> VoyageOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class VoyageStopEditViewModel : VoyageStopCreateViewModel { [Required] public Guid Id { get; set; } }

    public class VesselCrewMemberListItemViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string CrewRole { get; set; } = string.Empty;
        public string? SeamanBookNumber { get; set; }
        public string? CertificateNumber { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class VesselCrewMemberCreateViewModel
    {
        [Required, StringLength(150)] public string FullName { get; set; } = string.Empty;
        [Required] public VesselCrewRole CrewRole { get; set; } = VesselCrewRole.Other;
        [StringLength(50)] public string? SeamanBookNumber { get; set; }
        [StringLength(50)] public string? CertificateNumber { get; set; }
        [StringLength(30)] public string? Phone { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class VesselCrewMemberEditViewModel : VesselCrewMemberCreateViewModel { [Required] public Guid Id { get; set; } }

    public class CrewAssignmentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid VoyageId { get; set; }
        public Guid VesselCrewMemberId { get; set; }
        public string VoyageNumber { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string CrewMemberName { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
        public DateTime? AssignedAtUtc { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class CrewAssignmentCreateViewModel
    {
        [Required] public Guid VoyageId { get; set; }
        [Required] public Guid VesselCrewMemberId { get; set; }
        [Required] public VesselCrewRole AssignedRole { get; set; } = VesselCrewRole.Other;
        public DateTime? AssignedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> VoyageOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> CrewMemberOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class CrewAssignmentEditViewModel : CrewAssignmentCreateViewModel { [Required] public Guid Id { get; set; } }
}
