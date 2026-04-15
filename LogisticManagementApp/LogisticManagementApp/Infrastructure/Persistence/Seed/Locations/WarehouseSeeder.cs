using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Locations
{
    public static class WarehouseSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Warehouses.AnyAsync())
                return;

            var locations = await db.Locations
                .Where(x => x.LocationType == LocationType.Warehouse)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var warehouses = new List<Warehouse>();

            foreach (var location in locations)
            {
                warehouses.Add(new Warehouse
                {
                    Id = Guid.NewGuid(),
                    LocationId = location.Id,
                    WarehouseType = WarehouseType.General,
                    CapacityCubicMeters = 25000m,
                    CutOffTime = new TimeSpan(17, 0, 0),
                    IsBonded = location.Code.Contains("HUB"),
                    OperatingHours = "Mon-Fri 08:00-18:00",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Warehouses.AddRangeAsync(warehouses);
            await db.SaveChangesAsync();
        }
    }
}
