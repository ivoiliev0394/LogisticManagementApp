using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Party
{
    public class CompanyShipmentPartyEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Компания")]
        public Guid CompanyId { get; set; }

        [Required]
        [Display(Name = "Роля")]
        public PartyRole Role { get; set; }

        [Display(Name = "Контакт")]
        public Guid? CompanyContactId { get; set; }

        public IEnumerable<SelectListItem> CompanyOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ContactOptions { get; set; } = new List<SelectListItem>();
    }
}
