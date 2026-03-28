using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Documents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Compliance
{
    public class DGDocument : BaseEntity
    {
        [Required]
        public Guid DangerousGoodsDeclarationId { get; set; }

        [ForeignKey(nameof(DangerousGoodsDeclarationId))]
        public DangerousGoodsDeclaration DangerousGoodsDeclaration { get; set; } = null!;

        [Required]
        public Guid FileResourceId { get; set; }

        [ForeignKey(nameof(FileResourceId))]
        public FileResource FileResource { get; set; } = null!;

        [MaxLength(100)]
        public string? DocumentName { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
