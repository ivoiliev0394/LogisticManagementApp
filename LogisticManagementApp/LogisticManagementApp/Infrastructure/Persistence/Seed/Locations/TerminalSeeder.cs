using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Locations
{
    public static class TerminalSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Terminals.AnyAsync())
                return;

            var locations = await db.Locations
                .Where(x =>
                    x.LocationType == LocationType.Port ||
                    x.LocationType == LocationType.Airport ||
                    x.LocationType == LocationType.Terminal ||
                    x.LocationType == LocationType.RailTerminal)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var terminals = new List<Terminal>();

            foreach (var location in locations)
            {
                var terminalType = TerminalType.CargoTerminal;

                if (location.LocationType == LocationType.Port)
                    terminalType = TerminalType.SeaPortTerminal;
                else if (location.LocationType == LocationType.Airport)
                    terminalType = TerminalType.AirportTerminal;
                else if (location.LocationType == LocationType.RailTerminal)
                    terminalType = TerminalType.RailTerminal;
                else if (location.Code.Contains("RIVER"))
                    terminalType = TerminalType.RiverTerminal;

                terminals.Add(new Terminal
                {
                    Id = Guid.NewGuid(),
                    LocationId = location.Id,
                    TerminalType = terminalType,
                    TerminalCode = location.Code,
                    CapacityCbm = 40000m,
                    CapacityTons = 12000m,
                    IsBonded = true,
                    IsActive = true,
                    OperatingHours = "24/7",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Terminals.AddRangeAsync(terminals);
            await db.SaveChangesAsync();
        }
    }
}
