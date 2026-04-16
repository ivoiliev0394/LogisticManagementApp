namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Statuses
{
    public class CompanyShipmentStatusHistoryViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public IList<CompanyShipmentStatusHistoryItemViewModel> Items { get; set; }
            = new List<CompanyShipmentStatusHistoryItemViewModel>();
    }

    public class CompanyShipmentStatusHistoryItemViewModel
    {
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAtUtc { get; set; }
        public string? Reason { get; set; }
    }
}
