namespace LogisticManagementApp.Models.CompanyPortal.Reports
{
    public class CompanyReportsIndexViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
    }

    public class CompanyOperationalReportViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }
        public string SourceName { get; set; } = "LogisticManagementApp";
        public int OrdersCount { get; set; }
        public int ShipmentsCount { get; set; }
        public int ActiveShipmentsCount { get; set; }
        public int InvoicesCount { get; set; }
        public int RoutesCount { get; set; }
        public int BookingsCount { get; set; }
        public IReadOnlyList<CompanyOperationalOrderRowViewModel> RecentOrders { get; set; } = Array.Empty<CompanyOperationalOrderRowViewModel>();
    }

    public class CompanyOperationalOrderRowViewModel
    {
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string PickupDate { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }

    public class CompanyAssetsReportViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }
        public string SourceName { get; set; } = "LogisticManagementApp";
        public int VesselsCount { get; set; }
        public int VehiclesCount { get; set; }
        public int AircraftCount { get; set; }
        public int TrainsCount { get; set; }
        public int ContainersCount { get; set; }
        public int TripsCount { get; set; }
        public int VoyagesCount { get; set; }
        public int FlightsCount { get; set; }
        public int RailMovementsCount { get; set; }
        public IReadOnlyList<CompanyAssetRowViewModel> LatestAssets { get; set; } = Array.Empty<CompanyAssetRowViewModel>();
    }

    public class CompanyAssetRowViewModel
    {
        public string Mode { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CompanyFinanceReportViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }
        public string SourceName { get; set; } = "LogisticManagementApp";
        public int InvoicesCount { get; set; }
        public int CreditNotesCount { get; set; }
        public int PaymentsCount { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal CreditIssued { get; set; }
        public IReadOnlyList<CompanyFinanceInvoiceRowViewModel> LatestInvoices { get; set; } = Array.Empty<CompanyFinanceInvoiceRowViewModel>();
        public IReadOnlyList<CompanyFinancePaymentRowViewModel> LatestPayments { get; set; } = Array.Empty<CompanyFinancePaymentRowViewModel>();
    }

    public class CompanyFinanceInvoiceRowViewModel
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }

    public class CompanyFinancePaymentRowViewModel
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }
}
