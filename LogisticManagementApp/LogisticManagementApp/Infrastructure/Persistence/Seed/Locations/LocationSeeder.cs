using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Locations
{
    public static class LocationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Locations.AnyAsync())
                return;

            var addresses = await db.Addresses.OrderBy(x => x.City).Take(15).ToListAsync();

            var data = new List<(string Code, string Name, LocationType Type)>
        {
            ("SOF-HUB", "Sofia Central Hub", LocationType.Warehouse),
            ("PDV-WH", "Plovdiv Distribution Center", LocationType.Warehouse),
            ("VAR-PORT", "Varna Port Terminal", LocationType.Port),
            ("BOJ-PORT", "Burgas Port Terminal", LocationType.Port),
            ("RSE-RIVER", "Ruse River Terminal", LocationType.Terminal),
            ("BUH-HUB", "Bucharest Logistics Hub", LocationType.Warehouse),
            ("SKG-AIR", "Thessaloniki Cargo Airport", LocationType.Airport),
            ("IST-PORT", "Istanbul Freight Port", LocationType.Port),
            ("BEG-RAIL", "Belgrade Rail Terminal", LocationType.RailTerminal),
            ("HAM-PORT", "Hamburg Container Port", LocationType.Port),
            ("RTM-PORT", "Rotterdam Mega Terminal", LocationType.Port),
            ("MRS-PORT", "Marseille Sea Hub", LocationType.Port),
            ("GOA-PORT", "Genoa Maritime Terminal", LocationType.Port),
            ("VIE-HUB", "Vienna Inland Hub", LocationType.Warehouse),
            ("BUD-RAIL", "Budapest Rail Cargo Center", LocationType.RailTerminal)
        };

            var locations = new List<Location>();

            for (int i = 0; i < 15; i++)
            {
                locations.Add(new Location
                {
                    Id = Guid.NewGuid(),
                    Code = data[i].Code,
                    Name = data[i].Name,
                    LocationType = data[i].Type,
                    AddressId = addresses[i].Id,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Locations.AddRangeAsync(locations);
            await db.SaveChangesAsync();
        }
    }
}
