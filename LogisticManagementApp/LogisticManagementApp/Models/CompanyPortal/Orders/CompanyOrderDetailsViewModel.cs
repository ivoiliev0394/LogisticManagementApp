namespace LogisticManagementApp.Models.CompanyPortal.Orders
{
    public class CompanyOrderDetailsViewModel
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? RequestedPickupDateUtc { get; set; }
        public string? CustomerReference { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanSubmit { get; set; }
        public bool CanConfirm { get; set; }
        public bool CanStartProgress { get; set; }
        public bool CanComplete { get; set; }
        public bool CanCancel { get; set; }
        public bool CanManageLines { get; set; }

        public Guid? ShipmentId { get; set; }
        public string? ShipmentNo { get; set; }

        public List<CompanyOrderLineItemViewModel> Lines { get; set; } = new();
        public List<CompanyOrderStatusHistoryItemViewModel> StatusHistory { get; set; } = new();
    }

    public class CompanyOrderLineItemViewModel
    {
        public Guid Id { get; set; }
        public int LineNo { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string? QuantityUnit { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
        public bool IsDangerousGoods { get; set; }
        public string? HsCode { get; set; }
        public string? OriginCountry { get; set; }
    }

    public class CompanyOrderStatusHistoryItemViewModel
    {
        public Guid Id { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAtUtc { get; set; }
        public string? Reason { get; set; }
    }
}
