namespace LogisticManagementApp.Models.Home
{
    public class HomeViewModel
    {
        public string? TrackingNumber { get; set; }

        public string? ShipmentNo { get; set; }
        public string? Status { get; set; }
        public string? CurrentLocation { get; set; }
        public string? LastUpdate { get; set; }
        public string? Details { get; set; }
        public bool TrackingFound { get; set; }

        public List<string> SupportedCountries { get; set; } = new();
        public List<string> CourierCompanies { get; set; } = new();

        public List<TrackingEventViewModel> TrackingEvents { get; set; } = new();
    }
}
