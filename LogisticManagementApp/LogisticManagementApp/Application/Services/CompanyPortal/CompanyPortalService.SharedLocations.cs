using LogisticManagementApp.Models.CompanyPortal.SharedLocations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanySharedLocationsHomeViewModel> GetSharedLocationsHomeAsync() =>
            Task.FromResult(
                new CompanySharedLocationsHomeViewModel
                {
                    Cards = new[]
                    {
                        new SharedLocationCardViewModel
                        {
                            Title = "Addresses",
                            Description = "Общи адреси, използвани в company portal.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Addresses)
                        },
                        new SharedLocationCardViewModel
                        {
                            Title = "Locations",
                            Description = "Общи operational locations за routing, shipments и assets.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Locations)
                        },
                        new SharedLocationCardViewModel
                        {
                            Title = "Warehouses",
                            Description = "Складови локации и капацитетни данни.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Warehouses)
                        },
                        new SharedLocationCardViewModel
                        {
                            Title = "Terminals",
                            Description = "Терминали, свързани с locations.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Terminals)
                        },
                        new SharedLocationCardViewModel
                        {
                            Title = "Docks",
                            Description = "Докове към warehouses и locations.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Docks)
                        }
                    }
                });

        public async Task<IEnumerable<AddressListItemViewModel>> GetAddressesAsync() =>
            await _dbContext.Addresses
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Country)
                .ThenBy(x => x.City)
                .ThenBy(x => x.Street)
                .Select(x => new AddressListItemViewModel
                {
                    Id = x.Id,
                    Country = x.Country,
                    Region = x.Region,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    Street = x.Street,
                    Building = x.Building,
                    Apartment = x.Apartment,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<IEnumerable<LocationListItemViewModel>> GetLocationsAsync() =>
            await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new LocationListItemViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LocationType = x.LocationType.ToString(),
                    AddressDisplay = x.Address.Country + ", " + x.Address.City + ", " + x.Address.Street,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<IEnumerable<WarehouseListItemViewModel>> GetWarehousesAsync() =>
            await _dbContext.Warehouses
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Location.Name)
                .Select(x => new WarehouseListItemViewModel
                {
                    Id = x.Id,
                    LocationDisplay = x.Location.Name + " (" + x.Location.Code + ")",
                    WarehouseType = x.WarehouseType.ToString(),
                    CapacityCubicMeters = x.CapacityCubicMeters,
                    CutOffTime = x.CutOffTime,
                    IsBonded = x.IsBonded,
                    OperatingHours = x.OperatingHours,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<IEnumerable<TerminalListItemViewModel>> GetTerminalsAsync() =>
            await _dbContext.Terminals
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Location.Name)
                .Select(x => new TerminalListItemViewModel
                {
                    Id = x.Id,
                    LocationDisplay = x.Location.Name + " (" + x.Location.Code + ")",
                    TerminalType = x.TerminalType.ToString(),
                    TerminalCode = x.TerminalCode,
                    CapacityCbm = x.CapacityCbm,
                    CapacityTons = x.CapacityTons,
                    IsBonded = x.IsBonded,
                    IsActive = x.IsActive,
                    OperatingHours = x.OperatingHours,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<IEnumerable<DockListItemViewModel>> GetDocksAsync() =>
            await _dbContext.Docks
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new DockListItemViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    DockType = x.DockType.ToString(),
                    Status = x.Status.ToString(),
                    ParentDisplay = x.Warehouse != null
                        ? "Warehouse: " + x.Warehouse.Location.Name + " (" + x.Warehouse.Location.Code + ")"
                        : x.Location != null
                            ? "Location: " + x.Location.Name + " (" + x.Location.Code + ")"
                            : "-",
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
    }
}