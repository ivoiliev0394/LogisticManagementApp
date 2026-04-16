using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Documents
{
    public class FileResourceListItemViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string StorageKey { get; set; } = string.Empty;
        public DateTime UploadedAtUtc { get; set; }
    }

    public class CompanyDocumentListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? DocumentNo { get; set; }
        public DateTime? IssuedAtUtc { get; set; }
        public string? IssuedByCompanyName { get; set; }
        public string? Notes { get; set; }
    }

    public class DocumentVersionListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentNo { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int VersionNo { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string? ChangeDescription { get; set; }
    }

    public class DocumentTemplateListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TemplateType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsGlobal { get; set; }
        public string? Notes { get; set; }
    }

    public class DocumentCreateViewModel
    {
        [Required] public Guid ShipmentId { get; set; }
        [Required] public DocumentType DocumentType { get; set; } = DocumentType.Other;
        [Required] public Guid FileResourceId { get; set; }
        [StringLength(50)] public string? DocumentNo { get; set; }
        public DateTime? IssuedAtUtc { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> ShipmentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> FileResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class DocumentEditViewModel : DocumentCreateViewModel { [Required] public Guid Id { get; set; } }

    public class DocumentVersionCreateViewModel
    {
        [Required] public Guid DocumentId { get; set; }
        [Required] public Guid FileResourceId { get; set; }
        [Range(1, int.MaxValue)] public int VersionNo { get; set; } = 1;
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        [StringLength(300)] public string? ChangeDescription { get; set; }
        public IEnumerable<SelectListItem> DocumentOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> FileResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class DocumentVersionEditViewModel : DocumentVersionCreateViewModel { [Required] public Guid Id { get; set; } }

    public class DocumentTemplateCreateViewModel
    {
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        [Required] public DocumentTemplateType TemplateType { get; set; } = DocumentTemplateType.Other;
        [Required] public Guid FileResourceId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(300)] public string? Notes { get; set; }
        public IEnumerable<SelectListItem> FileResourceOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
    public class DocumentTemplateEditViewModel : DocumentTemplateCreateViewModel { [Required] public Guid Id { get; set; } }
}
