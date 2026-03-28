using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Documents
{
    public class Document : BaseEntity
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public DocumentType DocumentType { get; set; } = DocumentType.Other;

        [Required]
        public Guid FileResourceId { get; set; }

        [ForeignKey(nameof(FileResourceId))]
        public FileResource FileResource { get; set; } = null!;

        [MaxLength(50)]
        public string? DocumentNo { get; set; }

        public DateTime? IssuedAtUtc { get; set; }

        /// <summary>
        /// Кой го е издал (по желание).
        /// </summary>
        public Guid? IssuedByCompanyId { get; set; }

        [ForeignKey(nameof(IssuedByCompanyId))]
        public Company? IssuedByCompany { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
