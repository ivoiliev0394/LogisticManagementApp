using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Routes
{
    public static class RoutePlanShipmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RoutePlanShipments.AnyAsync())
                return;

            var routePlans = await db.RoutePlans
                .OrderBy(rp => rp.PlanDateUtc)
                .Take(15)
                .ToListAsync();

            var shipments = await db.Shipments
                .OrderBy(s => s.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            if (!routePlans.Any() || !shipments.Any())
                return;

            var count = Math.Min(routePlans.Count, shipments.Count);
            var list = new List<RoutePlanShipment>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new RoutePlanShipment
                {
                    Id = Guid.NewGuid(),
                    RoutePlanId = routePlans[i].Id,
                    ShipmentId = shipments[i].Id,
                    Priority = i + 1,
                    Notes = "Shipment assigned to route plan",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RoutePlanShipments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}