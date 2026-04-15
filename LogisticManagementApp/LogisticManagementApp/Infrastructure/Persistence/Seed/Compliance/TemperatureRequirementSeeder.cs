using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Compliance
{
    public static class TemperatureRequirementSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TemperatureRequirements.AnyAsync())
                return;

            var shipments = await db.Shipments
                .OrderBy(s => s.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            if (!shipments.Any())
                return;

            var list = new List<TemperatureRequirement>();

            for (int i = 0; i < shipments.Count; i++)
            {
                list.Add(new TemperatureRequirement
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    MinTemperatureCelsius = 2,
                    MaxTemperatureCelsius = 8,
                    RequiresTemperatureMonitoring = true,
                    TemperatureUnit = "Celsius",
                    Notes = "Cold chain requirement",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.TemperatureRequirements.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}