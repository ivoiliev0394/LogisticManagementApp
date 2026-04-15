using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Locations
{
    public static class DockSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Docks.AnyAsync())
                return;

            var warehouses = await db.Warehouses.OrderBy(x => x.Id).ToListAsync();
            var terminals = await db.Terminals.OrderBy(x => x.Id).ToListAsync();

            var docks = new List<Dock>();

            foreach (var warehouse in warehouses.Take(8))
            {
                docks.Add(new Dock
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = warehouse.Id,
                    Code = $"WH-DOCK-{docks.Count + 1:00}",
                    DockType = DockType.LoadingDock,
                    Status = DockStatus.Available,
                    MaxWeightKg = 18000m,
                    MaxVolumeCbm = 90m,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            foreach (var terminal in terminals.Take(7))
            {
                docks.Add(new Dock
                {
                    Id = Guid.NewGuid(),
                    LocationId = terminal.LocationId,
                    Code = $"TRM-GATE-{docks.Count + 1:00}",
                    DockType = DockType.Gate,
                    Status = DockStatus.Available,
                    MaxWeightKg = 25000m,
                    MaxVolumeCbm = 120m,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Docks.AddRangeAsync(docks);
            await db.SaveChangesAsync();
        }
    }
}
