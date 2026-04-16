namespace LogisticManagementApp.Models.CompanyPortal.Orders
{
    public class CompanyOrdersViewModel
    {
        public List<CompanyOrderListItemViewModel> Orders { get; set; } = new();
    }

    public class CompanyOrderListItemViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? RequestedPickupDateUtc { get; set; }
        public string? CustomerReference { get; set; }
        public int LinesCount { get; set; }
        public Guid? ShipmentId { get; set; }
        public string? ShipmentNo { get; set; }
    }
}
