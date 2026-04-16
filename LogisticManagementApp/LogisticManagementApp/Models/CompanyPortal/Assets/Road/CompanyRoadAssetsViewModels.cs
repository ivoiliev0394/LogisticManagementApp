using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Assets.Road
{
    public class RoadAssetCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyRoadHomeViewModel
    {
        public IEnumerable<RoadAssetCardViewModel> Cards { get; set; } = Enumerable.Empty<RoadAssetCardViewModel>();
    }

    public class VehicleListItemViewModel
    {
        public Guid Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal? MaxWeightKg { get; set; }
        public decimal? MaxVolumeCbm { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class VehicleCreateViewModel
    {
        [Required, StringLength(20)] public string RegistrationNumber { get; set; } = string.Empty;
        [Required] public VehicleType VehicleType { get; set; } = VehicleType.Truck;
        [StringLength(100)] public string? Brand { get; set; }
        [StringLength(100)] public string? Model { get; set; }
        [Range(0, double.MaxValue)] public decimal? MaxWeightKg { get; set; }
        [Range(0, double.MaxValue)] public decimal? MaxVolumeCbm { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class VehicleEditViewModel : VehicleCreateViewModel { [Required] public Guid Id { get; set; } }

    public class DriverListItemViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LicenseCategory { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class DriverCreateViewModel
    {
        [Required, StringLength(150)] public string FullName { get; set; } = string.Empty;
        [Required] public DriverLicenseCategory LicenseCategory { get; set; } = DriverLicenseCategory.B;
        [StringLength(50)] public string? LicenseNumber { get; set; }
        [StringLength(30)] public string? Phone { get; set; }
        [Required] public AssetStatus Status { get; set; } = AssetStatus.Available;
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
    }
    public class DriverEditViewModel : DriverCreateViewModel { [Required] public Guid Id { get; set; } }

    public class TripListItemViewModel
    {
        public Guid Id { get; set; }
        public string TripNo { get; set; } = string.Empty;
        public string? VehicleDisplay { get; set; }
        public string? DriverName { get; set; }
        public string? OriginLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class TripCreateViewModel
    {
        [Required, StringLength(50)] public string TripNo { get; set; } = string.Empty;
        public Guid? VehicleId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        [Required] public TripStatus Status { get; set; } = TripStatus.Planned;
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> VehicleOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> DriverOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class TripEditViewModel : TripCreateViewModel { [Required] public Guid Id { get; set; } }

    public class TripStopListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string TripNo { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class TripStopCreateViewModel
    {
        [Required] public Guid TripId { get; set; }
        [Required] public Guid LocationId { get; set; }
        [Range(1, int.MaxValue)] public int SequenceNumber { get; set; } = 1;
        public DateTime? PlannedArrivalUtc { get; set; }
        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> TripOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> LocationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class TripStopEditViewModel : TripStopCreateViewModel { [Required] public Guid Id { get; set; } }

    public class TripShipmentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string TripNo { get; set; } = string.Empty;
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string? ShipmentLegDisplay { get; set; }
        public string? PickupStopDisplay { get; set; }
        public string? DeliveryStopDisplay { get; set; }
        public int Priority { get; set; }
        public string? Notes { get; set; }
    }

    public class TripShipmentCreateViewModel
    {
        [Required] public Guid TripId { get; set; }
        [Required] public Guid ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid? PickupTripStopId { get; set; }
        public Guid? DeliveryTripStopId { get; set; }
        [Range(1, int.MaxValue)] public int Priority { get; set; } = 1;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> TripOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ShipmentLegOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> TripStopOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class TripShipmentEditViewModel : TripShipmentCreateViewModel { [Required] public Guid Id { get; set; } }
}
