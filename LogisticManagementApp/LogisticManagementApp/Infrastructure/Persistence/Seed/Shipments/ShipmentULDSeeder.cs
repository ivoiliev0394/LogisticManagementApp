using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentULDSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentULDs.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var ulds = await db.ULDs.Take(15).ToListAsync();

            var list = new List<ShipmentULD>();

            for (int i = 0; i < shipments.Count && i < ulds.Count; i++)
            {
                list.Add(new ShipmentULD
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    UldId = ulds[i].Id,
                    GrossWeightKg = 2500m + (i * 100),
                    VolumeCbm = 8m + (i * 0.5m),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentULDs.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
