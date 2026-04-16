namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Party
{
    public class CompanyShipmentPartiesViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageParties { get; set; }

        public IList<CompanyShipmentPartyListItemViewModel> Parties { get; set; }
            = new List<CompanyShipmentPartyListItemViewModel>();
    }
}
