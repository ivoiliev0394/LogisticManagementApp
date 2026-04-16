using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments.ProofOfDelivery
{
    public class CompanyProofOfDeliveryEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Delivered at UTC")]
        public DateTime DeliveredAtUtc { get; set; }

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
