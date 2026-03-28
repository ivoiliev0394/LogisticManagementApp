using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Orders
{
    public class Order : BaseEntity
    {
        /// <summary>
        /// Човеко-четим номер (ORD-2026-000123). Не го правя auto тук – генерира се в service по-късно.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string OrderNo { get; set; } = null!;

        /// <summary>
        /// Клиентът, който прави поръчката.
        /// </summary>
        [Required]
        public Guid CustomerCompanyId { get; set; }

        [ForeignKey(nameof(CustomerCompanyId))]
        public Company CustomerCompany { get; set; } = null!;

        /// <summary>
        /// Конкретен клиентски профил към компанията, ако е избран.
        /// </summary>
        /// 
        public Guid? ClientProfileId { get; set; }
        [ForeignKey(nameof(ClientProfileId))]
        public ClientProfile? ClientProfile { get; set; }

        /// <summary>
        /// Адрес за взимане (pickup) и доставка (delivery) на ниво поръчка.
        /// По-късно shipment може да има свои адреси, но това е базовият заявен вариант.
        /// </summary>
        public Guid? PickupAddressId { get; set; }

        [ForeignKey(nameof(PickupAddressId))]
        public Address? PickupAddress { get; set; }

        public Guid? DeliveryAddressId { get; set; }

        [ForeignKey(nameof(DeliveryAddressId))]
        public Address? DeliveryAddress { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        [Required]
        public OrderPriority Priority { get; set; } = OrderPriority.Normal;

        /// <summary>
        /// Клиентът иска взимане на/след тази дата.
        /// </summary>
        public DateTime? RequestedPickupDateUtc { get; set; }

        /// <summary>
        /// Клиентска референция (напр. PO номер).
        /// </summary>
        [MaxLength(50)]
        public string? CustomerReference { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }
}
