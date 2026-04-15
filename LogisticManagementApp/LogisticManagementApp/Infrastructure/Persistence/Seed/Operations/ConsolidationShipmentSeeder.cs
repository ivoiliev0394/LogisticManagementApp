using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations
{
    public static class ConsolidationShipmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ConsolidationShipments.AnyAsync())
                return;

            var consolidations = await db.Consolidations.Take(15).ToListAsync();
            var shipments = await db.Shipments.Take(15).ToListAsync();

            var list = new List<ConsolidationShipment>();

            for (int i = 0; i < 15 && i < consolidations.Count && i < shipments.Count; i++)
            {
                list.Add(new ConsolidationShipment
                {
                    Id = Guid.NewGuid(),
                    ConsolidationId = consolidations[i].Id,
                    ShipmentId = shipments[i].Id,
                    AllocatedWeightKg = 200m + (i * 10),
                    AllocatedVolumeCbm = 1.5m + (i * 0.1m),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ConsolidationShipments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
