namespace LogisticManagementApp.Models.CompanyPortal.SharedLocations
{
    public class SharedLocationCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
    }

    public class CompanySharedLocationsHomeViewModel
    {
        public IEnumerable<SharedLocationCardViewModel> Cards { get; set; } = Enumerable.Empty<SharedLocationCardViewModel>();
    }

    public class AddressListItemViewModel
    {
        public Guid Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Street { get; set; } = string.Empty;
        public string? Building { get; set; }
        public string? Apartment { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Notes { get; set; }
    }

    public class LocationListItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public string AddressDisplay { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class WarehouseListItemViewModel
    {
        public Guid Id { get; set; }
        public string LocationDisplay { get; set; } = string.Empty;
        public string WarehouseType { get; set; } = string.Empty;
        public decimal? CapacityCubicMeters { get; set; }
        public TimeSpan? CutOffTime { get; set; }
        public bool IsBonded { get; set; }
        public string? OperatingHours { get; set; }
        public string? Notes { get; set; }
    }

    public class TerminalListItemViewModel
    {
        public Guid Id { get; set; }
        public string LocationDisplay { get; set; } = string.Empty;
        public string TerminalType { get; set; } = string.Empty;
        public string? TerminalCode { get; set; }
        public decimal? CapacityCbm { get; set; }
        public decimal? CapacityTons { get; set; }
        public bool IsBonded { get; set; }
        public bool IsActive { get; set; }
        public string? OperatingHours { get; set; }
        public string? Notes { get; set; }
    }

    public class DockListItemViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DockType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ParentDisplay { get; set; } = string.Empty;
        public decimal? MaxWeightKg { get; set; }
        public decimal? MaxVolumeCbm { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
