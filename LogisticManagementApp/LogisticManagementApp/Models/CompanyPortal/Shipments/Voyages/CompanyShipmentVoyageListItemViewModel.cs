namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Voyages
{
    public class CompanyShipmentVoyageListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public Guid VoyageId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public string? BookingReference { get; set; }
        public string? Notes { get; set; }
    }
}
