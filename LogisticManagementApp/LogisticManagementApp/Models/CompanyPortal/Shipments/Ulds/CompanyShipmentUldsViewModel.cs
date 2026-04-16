namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Ulds
{
    public class CompanyShipmentUldsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageUlds { get; set; }

        public IList<CompanyShipmentUldListItemViewModel> Ulds { get; set; }
            = new List<CompanyShipmentUldListItemViewModel>();
    }
}
