using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class TariffSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Tariffs.AnyAsync())
                return;

            var serviceLevels = await db.ServiceLevels.Take(5).ToListAsync();
            var zones = await db.GeoZones.Take(3).ToListAsync();

            var tariffs = new List<Tariff>();

            foreach (var service in serviceLevels)
            {
                foreach (var zone in zones)
                {
                    tariffs.Add(new Tariff
                    {
                        Id = Guid.NewGuid(),
                        ServiceLevelId = service.Id,
                        GeoZoneId = zone.Id,
                        ValidFromUtc = DateTime.UtcNow.AddMonths(-1),
                        ValidToUtc = DateTime.UtcNow.AddYears(1),
                        Currency = "EUR",
                        IsActive = true,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }

            await db.Tariffs.AddRangeAsync(tariffs);
            await db.SaveChangesAsync();
        }
    }
}
