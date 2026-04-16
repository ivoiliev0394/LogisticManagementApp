namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Legs
{
    public class CompanyLegStatusHistoryItemViewModel
    {
        public Guid Id { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAtUtc { get; set; }
        public string? Reason { get; set; }
    }
}
