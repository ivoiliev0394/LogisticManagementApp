namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Trips
{
    public class CompanyShipmentTripsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageTrips { get; set; }

        public IList<CompanyShipmentTripListItemViewModel> Trips { get; set; }
            = new List<CompanyShipmentTripListItemViewModel>();
    }
}
