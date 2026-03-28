using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Documents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Orders
{
    public class OrderAttachment : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        [Required]
        public Guid FileResourceId { get; set; }

        [ForeignKey(nameof(FileResourceId))]
        public FileResource FileResource { get; set; } = null!;

        [MaxLength(100)]
        public string? AttachmentType { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
