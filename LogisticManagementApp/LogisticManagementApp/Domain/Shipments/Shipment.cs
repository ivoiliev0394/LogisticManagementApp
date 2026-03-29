using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Orders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Shipments
{
    public class Shipment : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string ShipmentNo { get; set; } = null!; // SHP-2026-000001

        /// <summary>
        /// Клиентът (фирмата), за която се изпълнява пратката.
        /// </summary>
        [Required]
        public Guid CustomerCompanyId { get; set; }

        [ForeignKey(nameof(CustomerCompanyId))]
        public Company CustomerCompany { get; set; } = null!;

        /// <summary>
        /// Връзка към поръчка (1 Order : N Shipments). Nullable за ръчни/външни shipments.
        /// </summary>
        public Guid? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        /// <summary>
        /// Адреси на ниво shipment (final). Това е важно за куриер/last mile.
        /// </summary>
        public Guid? SenderAddressId { get; set; }

        [ForeignKey(nameof(SenderAddressId))]
        public Address? SenderAddress { get; set; }

        public Guid? ReceiverAddressId { get; set; }

        [ForeignKey(nameof(ReceiverAddressId))]
        public Address? ReceiverAddress { get; set; }

        [Required]
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;

        [Required]
        public TransportMode PrimaryMode { get; set; } = TransportMode.Road;

        public Incoterm? Incoterm { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DeclaredValue { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; } // ISO (EUR, USD...)

        [MaxLength(50)]
        public string? CustomerReference { get; set; } // PO/Ref

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public ICollection<ShipmentParty> Parties { get; set; } = new List<ShipmentParty>();
        public ICollection<ShipmentLeg> Legs { get; set; } = new List<ShipmentLeg>();
    }
}
