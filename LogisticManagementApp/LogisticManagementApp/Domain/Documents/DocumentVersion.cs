using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Documents
{
    public class DocumentVersion : BaseEntity
    {
        [Required]
        public Guid DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document Document { get; set; } = null!;

        [Required]
        public Guid FileResourceId { get; set; }

        [ForeignKey(nameof(FileResourceId))]
        public FileResource FileResource { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int VersionNo { get; set; } = 1;

        [Required]
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? ChangeDescription { get; set; }
    }
}
