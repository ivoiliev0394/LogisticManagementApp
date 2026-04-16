namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Tags
{
    public class CompanyShipmentTagsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageTags { get; set; }

        public IList<CompanyShipmentTagListItemViewModel> Tags { get; set; }
            = new List<CompanyShipmentTagListItemViewModel>();
    }
}
