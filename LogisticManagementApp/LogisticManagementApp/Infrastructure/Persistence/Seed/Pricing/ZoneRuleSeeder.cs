using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class ZoneRuleSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ZoneRules.AnyAsync())
                return;

            var zones = await db.GeoZones.OrderBy(x => x.Code).Take(15).ToListAsync();

            var rules = new List<ZoneRule>();

            for (int i = 0; i < zones.Count; i++)
            {
                rules.Add(new ZoneRule
                {
                    Id = Guid.NewGuid(),
                    GeoZoneId = zones[i].Id,
                    Country = i < 5 ? "Bulgaria" :
              i < 10 ? "EU" : "International",
                    PostalCodeFrom = $"{i + 1:D2}000",
                    PostalCodeTo = $"{i + 1:D2}999",
                    City = $"City-{i + 1}",
                    Priority = i + 1,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ZoneRules.AddRangeAsync(rules);
            await db.SaveChangesAsync();
        }
    }
}
