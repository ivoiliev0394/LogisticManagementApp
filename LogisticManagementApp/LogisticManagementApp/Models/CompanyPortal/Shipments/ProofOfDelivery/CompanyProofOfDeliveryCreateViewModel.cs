using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.ProofOfDelivery
{
    public class CompanyProofOfDeliveryCreateViewModel
    {
        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Delivered at UTC")]
        public DateTime DeliveredAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(150)]
        [Display(Name = "Receiver name")]
        public string? ReceiverName { get; set; }

        [Display(Name = "Signature file resource id")]
        public Guid? SignatureFileResourceId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
