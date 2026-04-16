using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Assets.Rail
{
    public class RailAssetCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyRailHomeViewModel
    {
        public IEnumerable<RailAssetCardViewModel> Cards { get; set; }
            = Enumerable.Empty<RailAssetCardViewModel>();
    }

    public class TrainListItemViewModel
    {
        public Guid Id { get; set; }
        public string TrainNumber { get; set; } = string.Empty;
        public string TrainType { get; set; } = string.Empty;
        public decimal? MaxWeightKg { get; set; }
        public decimal? MaxVolumeCbm { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class TrainCreateViewModel
    {
        [Required, StringLength(50)]
        public string TrainNumber { get; set; } = string.Empty;

        [Required]
        public TrainType TrainType { get; set; } = TrainType.Freight;

        [Range(0, double.MaxValue)]
        public decimal? MaxWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxVolumeCbm { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class TrainEditViewModel : TrainCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RailCarListItemViewModel
    {
        public Guid Id { get; set; }
        public string RailCarNumber { get; set; } = string.Empty;
        public string RailCarType { get; set; } = string.Empty;
        public decimal? MaxWeightKg { get; set; }
        public decimal? MaxVolumeCbm { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class RailCarCreateViewModel
    {
        [Required, StringLength(50)]
        public string RailCarNumber { get; set; } = string.Empty;

        [Required]
        public RailCarType RailCarType { get; set; } = RailCarType.Boxcar;

        [Range(0, double.MaxValue)]
        public decimal? MaxWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxVolumeCbm { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class RailCarEditViewModel : RailCarCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RailServiceListItemViewModel
    {
        public Guid Id { get; set; }
        public string ServiceCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? OriginLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public int? EstimatedTransitDays { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class RailServiceCreateViewModel
    {
        [Required, StringLength(50)]
        public string ServiceCode { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }

        [Range(0, int.MaxValue)]
        public int? EstimatedTransitDays { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> LocationOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RailServiceEditViewModel : RailServiceCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RailMovementListItemViewModel
    {
        public Guid Id { get; set; }
        public string MovementNo { get; set; } = string.Empty;
        public string? TrainNumber { get; set; }
        public string? RailServiceDisplay { get; set; }
        public string? OriginLocation { get; set; }
        public string? DestinationLocation { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public string? Notes { get; set; }
    }

    public class RailMovementCreateViewModel
    {
        public Guid? TrainId { get; set; }
        public Guid? RailServiceId { get; set; }

        [Required, StringLength(50)]
        public string MovementNo { get; set; } = string.Empty;

        public Guid? OriginLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }

        [Required]
        public RailMovementStatus Status { get; set; } = RailMovementStatus.Planned;

        public DateTime? ScheduledDepartureUtc { get; set; }
        public DateTime? ScheduledArrivalUtc { get; set; }
        public DateTime? ActualDepartureUtc { get; set; }
        public DateTime? ActualArrivalUtc { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> TrainOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> RailServiceOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> LocationOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class RailMovementEditViewModel : RailMovementCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}