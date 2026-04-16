namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Legs
{
    public class CompanyShipmentLegDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;
        public int LegNo { get; set; }

        public string Mode { get; set; } = string.Empty;
        public string OriginLocation { get; set; } = string.Empty;
        public string DestinationLocation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }
        public DateTime? ATD_Utc { get; set; }
        public DateTime? ATA_Utc { get; set; }

        public string? CarrierReference { get; set; }
        public string? Notes { get; set; }

        public bool CanManageLeg { get; set; }

        public IList<CompanyLegStatusHistoryItemViewModel> StatusHistories { get; set; }
            = new List<CompanyLegStatusHistoryItemViewModel>();
    }
}
