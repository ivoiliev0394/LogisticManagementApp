namespace LogisticManagementApp.Models.Home
{
    public class TrackingEventViewModel
    {
        public string EventType { get; set; } = string.Empty;
        public string EventTime { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Details { get; set; }
    }
}
