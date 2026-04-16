namespace LogisticManagementApp.Models.CompanyPortal.Shipments.ProofOfDelivery
{
    public class CompanyProofOfDeliveryListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public DateTime DeliveredAtUtc { get; set; }
        public string? ReceiverName { get; set; }
        public string? Notes { get; set; }

        public Guid? SignatureFileResourceId { get; set; }
    }
}
