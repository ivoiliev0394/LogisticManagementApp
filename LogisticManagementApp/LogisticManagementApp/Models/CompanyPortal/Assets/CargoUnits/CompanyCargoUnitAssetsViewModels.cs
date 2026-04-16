using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Assets.CargoUnits
{
    public class CargoUnitAssetCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string? CreateActionName { get; set; }
    }

    public class CompanyCargoUnitHomeViewModel
    {
        public IEnumerable<CargoUnitAssetCardViewModel> Cards { get; set; }
            = Enumerable.Empty<CargoUnitAssetCardViewModel>();
    }

    public class ContainerListItemViewModel
    {
        public Guid Id { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public string ContainerType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public decimal? TareWeightKg { get; set; }
        public decimal? MaxGrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }

        public string? SealNumber { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class ContainerCreateViewModel
    {
        [Required, StringLength(20)]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        public ContainerType ContainerType { get; set; } = ContainerType.GP20;

        [Required]
        public ContainerStatus Status { get; set; } = ContainerStatus.Available;

        [Range(0, double.MaxValue)]
        public decimal? TareWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxGrossWeightKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? VolumeCbm { get; set; }

        [StringLength(50)]
        public string? SealNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class ContainerEditViewModel : ContainerCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class ContainerSealListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ContainerId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public string SealNumber { get; set; } = string.Empty;

        public DateTime? AppliedAtUtc { get; set; }
        public string? AppliedBy { get; set; }

        public DateTime? RemovedAtUtc { get; set; }
        public string? RemovedBy { get; set; }

        public bool IsActiveSeal { get; set; }
        public string? Notes { get; set; }
    }

    public class ContainerSealCreateViewModel
    {
        [Required]
        public Guid ContainerId { get; set; }

        [Required, StringLength(50)]
        public string SealNumber { get; set; } = string.Empty;

        public DateTime? AppliedAtUtc { get; set; }

        [StringLength(150)]
        public string? AppliedBy { get; set; }

        public DateTime? RemovedAtUtc { get; set; }

        [StringLength(150)]
        public string? RemovedBy { get; set; }

        public bool IsActiveSeal { get; set; } = true;

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> ContainerOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }

    public class ContainerSealEditViewModel : ContainerSealCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}