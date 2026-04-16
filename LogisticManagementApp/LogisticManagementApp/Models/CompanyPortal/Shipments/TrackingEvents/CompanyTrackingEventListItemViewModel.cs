namespace LogisticManagementApp.Models.CompanyPortal.Shipments.TrackingEvents
{
    public class CompanyTrackingEventListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string EventType { get; set; } = string.Empty;
        public DateTime EventTimeUtc { get; set; }

        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        public string? Details { get; set; }
        public string? Source { get; set; }
    }
}
