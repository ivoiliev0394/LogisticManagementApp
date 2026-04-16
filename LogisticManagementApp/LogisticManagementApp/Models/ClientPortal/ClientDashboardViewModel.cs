namespace LogisticManagementApp.Models.ClientPortal
{
    public class ClientDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public int AddressCount { get; set; }
        public int OrderCount { get; set; }
        public int ActiveOrderCount { get; set; }
        public int ShipmentCount { get; set; }
        public int ActiveShipmentCount { get; set; }

        public List<ClientDashboardOrderItemViewModel> RecentOrders { get; set; } = new();
        public List<ClientDashboardShipmentItemViewModel> RecentShipments { get; set; } = new();
        public List<ClientAddressItemViewModel> Addresses { get; set; } = new();
    }

    public class ClientDashboardOrderItemViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? RequestedPickupDateUtc { get; set; }
        public Guid? ShipmentId { get; set; }
        public string? ShipmentNo { get; set; }
    }

    public class ClientDashboardShipmentItemViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ReceiverAddress { get; set; }
        public string? CustomerReference { get; set; }
        public string? LastTrackingEvent { get; set; }
        public DateTime? LastTrackingEventUtc { get; set; }
    }

    public class ClientAddressItemViewModel
    {
        public Guid Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; }

        public string DisplayText => string.IsNullOrWhiteSpace(PostalCode)
            ? $"{Country}, {City}, {Street}"
            : $"{Country}, {City}, {Street}, {PostalCode}";
    }
}
