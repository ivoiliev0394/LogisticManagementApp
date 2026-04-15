using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentTripSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentTrips.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var trips = await db.Trips.Take(15).ToListAsync();

            var list = new List<ShipmentTrip>();

            for (int i = 0; i < shipments.Count && i < trips.Count; i++)
            {
                list.Add(new ShipmentTrip
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    TripId = trips[i].Id,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentTrips.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
