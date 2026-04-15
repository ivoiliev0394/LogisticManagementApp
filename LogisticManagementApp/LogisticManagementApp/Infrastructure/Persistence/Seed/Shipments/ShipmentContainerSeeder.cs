using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentContainerSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentContainers.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var containers = await db.Containers.Take(15).ToListAsync();

            var list = new List<ShipmentContainer>();

            for (int i = 0; i < shipments.Count && i < containers.Count; i++)
            {
                list.Add(new ShipmentContainer
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    ContainerId = containers[i].Id,
                    GrossWeightKg = 18000m + (i * 500),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentContainers.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
