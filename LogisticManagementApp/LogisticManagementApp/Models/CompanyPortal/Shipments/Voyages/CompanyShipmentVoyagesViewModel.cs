namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Voyages
{
    public class CompanyShipmentVoyagesViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageVoyages { get; set; }

        public IList<CompanyShipmentVoyageListItemViewModel> Voyages { get; set; }
            = new List<CompanyShipmentVoyageListItemViewModel>();
    }
}
