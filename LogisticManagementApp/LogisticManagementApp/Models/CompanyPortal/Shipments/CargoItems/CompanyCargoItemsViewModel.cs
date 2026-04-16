namespace LogisticManagementApp.Models.CompanyPortal.Shipments.CargoItems
{
    public class CompanyCargoItemsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageCargoItems { get; set; }

        public IList<CompanyCargoItemListItemViewModel> CargoItems { get; set; }
            = new List<CompanyCargoItemListItemViewModel>();
    }
}
