using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Orders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Orders
{
    public class OrderStatusHistory : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        [Required]
        public OrderStatus OldStatus { get; set; }

        [Required]
        public OrderStatus NewStatus { get; set; }

        [Required]
        public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
