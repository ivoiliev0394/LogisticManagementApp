using LogisticManagementApp.Domain.Enums.Billing;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentLegSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentLegs.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var statuses = Enum.GetValues<LegStatus>();
            int statusIndex = 1;

            var legs = new List<ShipmentLeg>();

            for (int i = 0; i < shipments.Count; i++)
            {
                legs.Add(new ShipmentLeg
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    LegNo = 1,
                    Mode = shipments[i].PrimaryMode,
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    Status = (LegStatus)statusIndex,
                    ETD_Utc = DateTime.UtcNow.AddDays(i + 1),
                    ETA_Utc = DateTime.UtcNow.AddDays(i + 3),
                    CarrierReference = $"CAR-{8000 + i}",
                    Notes = $"Primary leg for {shipments[i].ShipmentNo}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (statusIndex < statuses.Length) statusIndex++;
                else statusIndex = 1;
            }

            await db.ShipmentLegs.AddRangeAsync(legs);
            await db.SaveChangesAsync();
        }
    }
}
