using LogisticManagementApp.Domain.Enums.Orders;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Orders
{
    public class CompanyOrderCreateViewModel
    {
        [Display(Name = "Order number")]
        public string OrderNo { get; set; } = string.Empty;

        [Display(Name = "Pickup адрес")]
        public Guid? PickupAddressId { get; set; }

        [Display(Name = "Delivery адрес")]
        public Guid? DeliveryAddressId { get; set; }

        [Required]
        [Display(Name = "Приоритет")]
        public OrderPriority Priority { get; set; } = OrderPriority.Normal;

        [Display(Name = "Желана pickup дата")]
        public DateTime? RequestedPickupDateUtc { get; set; }

        [MaxLength(50)]
        [Display(Name = "Клиентска референция")]
        public string? CustomerReference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Бележки")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> AddressOptions { get; set; } = new List<SelectListItem>();
    }
}
