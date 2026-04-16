namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Legs
{
    public class CompanyShipmentLegsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageLegs { get; set; }

        public IList<CompanyShipmentLegListItemViewModel> Legs { get; set; }
            = new List<CompanyShipmentLegListItemViewModel>();
    }
}
