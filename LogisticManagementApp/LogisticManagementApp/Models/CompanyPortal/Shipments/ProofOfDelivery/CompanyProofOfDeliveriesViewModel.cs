namespace LogisticManagementApp.Models.CompanyPortal.Shipments.ProofOfDelivery
{
    public class CompanyProofOfDeliveriesViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageProofOfDeliveries { get; set; }
        public bool CanCreateProofOfDelivery { get; set; }

        public IList<CompanyProofOfDeliveryListItemViewModel> ProofOfDeliveries { get; set; }
            = new List<CompanyProofOfDeliveryListItemViewModel>();
    }
}
