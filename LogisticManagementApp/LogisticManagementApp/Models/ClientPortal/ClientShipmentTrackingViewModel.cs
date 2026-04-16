namespace LogisticManagementApp.Models.ClientPortal
{
    public class ClientShipmentTrackingViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CurrentLocation { get; set; }
        public string? LastUpdate { get; set; }
        public string? Details { get; set; }

        public List<ClientTrackingEventViewModel> TrackingEvents { get; set; } = new();
    }

    public class ClientTrackingEventViewModel
    {
        public string EventType { get; set; } = string.Empty;
        public string EventTime { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Details { get; set; }
    }

}
