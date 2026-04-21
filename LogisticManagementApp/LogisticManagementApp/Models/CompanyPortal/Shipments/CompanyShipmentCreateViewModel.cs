using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments
{
    public class CompanyShipmentCreateViewModel
    {
        [Display(Name = "Shipment number")]
        public string ShipmentNo { get; set; } = string.Empty;
        [Display(Name = "Поръчка")]
        public Guid? OrderId { get; set; }

        [Display(Name = "Sender address")]
        public Guid? SenderAddressId { get; set; }

        [Display(Name = "Receiver address")]
        public Guid? ReceiverAddressId { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;

        [Required]
        [Display(Name = "Основен транспорт")]
        public TransportMode PrimaryMode { get; set; } = TransportMode.Road;

        [Display(Name = "Incoterm")]
        public Incoterm? Incoterm { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Declared value")]
        public decimal? DeclaredValue { get; set; }

        [MaxLength(3)]
        [Display(Name = "Валута")]
        public string? Currency { get; set; }

        [MaxLength(50)]
        [Display(Name = "Клиентска референция")]
        public string? CustomerReference { get; set; }

        [MaxLength(500)]
        [Display(Name = "Бележки")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> OrderOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AddressOptions { get; set; } = new List<SelectListItem>();
    }
}
