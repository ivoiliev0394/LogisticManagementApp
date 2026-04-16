namespace LogisticManagementApp.Models.CompanyPortal.Shipments.TrackingEvents
{
    public class CompanyTrackingEventsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageTrackingEvents { get; set; }

        public IList<CompanyTrackingEventListItemViewModel> TrackingEvents { get; set; }
            = new List<CompanyTrackingEventListItemViewModel>();
    }
}
