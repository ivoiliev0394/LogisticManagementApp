namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Trips
{
    public class CompanyShipmentTripListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public Guid TripId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public string TripDisplay { get; set; } = string.Empty;
        public string? ShipmentLegDisplay { get; set; }
        public string? Notes { get; set; }
    }
}
