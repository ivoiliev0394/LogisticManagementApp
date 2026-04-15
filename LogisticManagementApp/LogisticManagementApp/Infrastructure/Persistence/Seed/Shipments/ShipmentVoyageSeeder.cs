using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentVoyageSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentVoyages.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var voyages = await db.Voyages.Take(15).ToListAsync();

            var list = new List<ShipmentVoyage>();

            for (int i = 0; i < shipments.Count && i < voyages.Count; i++)
            {
                list.Add(new ShipmentVoyage
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    VoyageId = voyages[i].Id,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentVoyages.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
