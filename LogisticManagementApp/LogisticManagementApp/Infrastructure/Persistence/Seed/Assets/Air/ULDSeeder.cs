using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class ULDSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ULDs.AnyAsync())
                return;

            var ulds = new List<ULD>();

            for (int i = 0; i < 15; i++)
            {
                ulds.Add(new ULD
                {
                    Id = Guid.NewGuid(),
                    UldNumber = $"AKE{i:000}",
                    MaxGrossWeightKg = 1588m,
                    TareWeightKg = 82m,
                    VolumeCbm = 4.5m,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ULDs.AddRangeAsync(ulds);
            await db.SaveChangesAsync();
        }
    }
}
