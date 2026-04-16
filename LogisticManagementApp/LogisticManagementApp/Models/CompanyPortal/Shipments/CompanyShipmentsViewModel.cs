namespace LogisticManagementApp.Models.CompanyPortal.Shipments
{
    public class CompanyShipmentsViewModel
    {
        public List<CompanyShipmentListItemViewModel> Shipments { get; set; } = new();
    }

    public class CompanyShipmentListItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PrimaryMode { get; set; } = string.Empty;
        public string? OrderNo { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? CustomerReference { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int LegsCount { get; set; }
        public int TrackingEventsCount { get; set; }
        public int PackagesCount { get; set; }
        public int CargoItemsCount { get; set; }
    }
}
