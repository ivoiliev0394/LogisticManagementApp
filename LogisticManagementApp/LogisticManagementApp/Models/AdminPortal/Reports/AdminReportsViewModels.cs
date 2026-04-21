namespace LogisticManagementApp.Models.AdminPortal.Reports
{
    public class AdminReportsIndexViewModel
    {
        public int TotalEntityGroups { get; set; }
        public int TotalEntities { get; set; }
    }

    public class AdminSystemOverviewReportViewModel
    {
        public DateTime GeneratedAtUtc { get; set; }
        public string SourceName { get; set; } = "LogisticManagementApp";
        public int CompaniesCount { get; set; }
        public int UsersCount { get; set; }
        public int OrdersCount { get; set; }
        public int ShipmentsCount { get; set; }
        public int InvoicesCount { get; set; }
        public int RoutesCount { get; set; }
        public int BookingsCount { get; set; }
        public int TrackedAssetsCount { get; set; }
        public IReadOnlyList<AdminSystemEntityCatalogItemViewModel> CatalogItems { get; set; } = Array.Empty<AdminSystemEntityCatalogItemViewModel>();
    }

    public class AdminSystemEntityCatalogItemViewModel
    {
        public string Group { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public int Fields { get; set; }
    }

    public class AdminEntityReportViewModel
    {
        public string EntityName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }
        public string SourceName { get; set; } = "LogisticManagementApp";
        public string? SearchTerm { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterValue { get; set; }
        public IReadOnlyList<string> Columns { get; set; } = Array.Empty<string>();
        public IReadOnlyList<Dictionary<string, string>> Rows { get; set; } = Array.Empty<Dictionary<string, string>>();
        public int TotalRows => Rows.Count;
    }
}
