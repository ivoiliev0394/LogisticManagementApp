using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Documents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class ProofOfDelivery : BaseEntity
    {
        /// <summary>
        /// 1:0..1 към Shipment (уникален FK ще го сложим с Fluent API по-късно).
        /// </summary>
        [Required]
        public Guid ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [Required]
        public DateTime DeliveredAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(150)]
        public string? ReceiverName { get; set; }

        /// <summary>
        /// Подпис/снимка като файл (по желание).
        /// </summary>
        public Guid? SignatureFileResourceId { get; set; }

        [ForeignKey(nameof(SignatureFileResourceId))]
        public FileResource? SignatureFileResource { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
