using LogisticManagementApp.Domain.Enums.Compliance;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Compliance
{
    public class DangerousGoodsDeclarationListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string? PackageNo { get; set; }
        public string UnNumber { get; set; } = string.Empty;
        public string ProperShippingName { get; set; } = string.Empty;
        public string HazardClass { get; set; } = string.Empty;
        public string? PackingGroup { get; set; }
        public decimal? NetQuantity { get; set; }
        public string? QuantityUnit { get; set; }
        public bool RequiresSpecialHandling { get; set; }
        public string? Notes { get; set; }
    }

    public class TemperatureRequirementListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public decimal? MinTemperatureCelsius { get; set; }
        public decimal? MaxTemperatureCelsius { get; set; }
        public bool RequiresTemperatureMonitoring { get; set; }
        public string? TemperatureUnit { get; set; }
        public string? Notes { get; set; }
    }

    public class ComplianceCheckListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string CheckType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CheckedAtUtc { get; set; }
        public string? CheckedBy { get; set; }
        public string? ResultDetails { get; set; }
        public string? Notes { get; set; }
    }

    public class DGDocumentListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid DangerousGoodsDeclarationId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string UnNumber { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? DocumentName { get; set; }
        public string? Notes { get; set; }
    }

    public class DangerousGoodsDeclarationCreateViewModel
    {
        [Required] public Guid ShipmentId { get; set; }
        public Guid? PackageId { get; set; }
        [Required, StringLength(20)] public string UnNumber { get; set; } = string.Empty;
        [Required, StringLength(200)] public string ProperShippingName { get; set; } = string.Empty;
        [Required] public HazardClass HazardClass { get; set; }
        public PackingGroup? PackingGroup { get; set; }
        [Range(0, double.MaxValue)] public decimal? NetQuantity { get; set; }
        [StringLength(50)] public string? QuantityUnit { get; set; }
        [StringLength(500)] public string? HandlingInstructions { get; set; }
        public bool RequiresSpecialHandling { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> PackageOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class DangerousGoodsDeclarationEditViewModel : DangerousGoodsDeclarationCreateViewModel { [Required] public Guid Id { get; set; } }

    public class TemperatureRequirementCreateViewModel
    {
        [Required] public Guid ShipmentId { get; set; }
        [Range(-100,100)] public decimal? MinTemperatureCelsius { get; set; }
        [Range(-100,100)] public decimal? MaxTemperatureCelsius { get; set; }
        public bool RequiresTemperatureMonitoring { get; set; } = true;
        [StringLength(100)] public string? TemperatureUnit { get; set; } = "Celsius";
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class TemperatureRequirementEditViewModel : TemperatureRequirementCreateViewModel { [Required] public Guid Id { get; set; } }

    public class ComplianceCheckCreateViewModel
    {
        [Required] public Guid ShipmentId { get; set; }
        [Required, StringLength(100)] public string CheckType { get; set; } = string.Empty;
        [Required] public ComplianceCheckStatus Status { get; set; } = ComplianceCheckStatus.Pending;
        public DateTime? CheckedAtUtc { get; set; }
        [StringLength(100)] public string? CheckedBy { get; set; }
        [StringLength(500)] public string? ResultDetails { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class ComplianceCheckEditViewModel : ComplianceCheckCreateViewModel { [Required] public Guid Id { get; set; } }

    public class DGDocumentCreateViewModel
    {
        [Required] public Guid DangerousGoodsDeclarationId { get; set; }
        [Required] public Guid FileResourceId { get; set; }
        [StringLength(100)] public string? DocumentName { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> DangerousGoodsDeclarationOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> FileResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class DGDocumentEditViewModel : DGDocumentCreateViewModel { [Required] public Guid Id { get; set; } }
}
