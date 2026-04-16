namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Containers
{
    public class CompanyShipmentContainersViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageContainers { get; set; }

        public IList<CompanyShipmentContainerListItemViewModel> Containers { get; set; }
            = new List<CompanyShipmentContainerListItemViewModel>();
    }
}
