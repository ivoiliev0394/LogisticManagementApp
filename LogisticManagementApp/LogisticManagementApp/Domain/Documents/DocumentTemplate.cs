using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Orders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Documents
{
    public class DocumentTemplate : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        public DocumentTemplateType TemplateType { get; set; } = DocumentTemplateType.Other;

        public Guid? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required]
        public Guid FileResourceId { get; set; }

        [ForeignKey(nameof(FileResourceId))]
        public FileResource FileResource { get; set; } = null!;

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
