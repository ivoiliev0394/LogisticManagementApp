using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations
{
    public class Booking : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; } = null!;

        /// <summary>
        /// Превозвачът/партньорът, към който е резервацията.
        /// </summary>
        public Guid? CarrierCompanyId { get; set; }

        [ForeignKey(nameof(CarrierCompanyId))]
        public Company? CarrierCompany { get; set; }

        /// <summary>
        /// По желание - booking може да е и директно към shipment.
        /// Ако е null, може да е само към booking legs / consolidation.
        /// </summary>
        public Guid? ShipmentId { get; set; }

        [ForeignKey(nameof(ShipmentId))]
        public Shipment? Shipment { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Draft;

        [Required]
        public TransportMode TransportMode { get; set; } = TransportMode.Road;

        public DateTime? RequestedAtUtc { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }

        [MaxLength(100)]
        public string? CarrierReference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<BookingLeg> Legs { get; set; } = new List<BookingLeg>();
    }
}
