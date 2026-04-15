using LogisticManagementApp.Domain.Assets.Road;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road
{
    public static class TripShipmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TripShipments.AnyAsync())
                return;

            var trips = await db.Trips.Take(15).ToListAsync();
            var shipments = await db.Shipments.Take(15).ToListAsync();
            var shipmentLegs = await db.ShipmentLegs.Take(15).ToListAsync();
            var tripStops = await db.TripStops.ToListAsync();

            var list = new List<TripShipment>();

            for (int i = 0; i < 15 && i < trips.Count && i < shipments.Count; i++)
            {
                var stopsForTrip = tripStops
                    .Where(x => x.TripId == trips[i].Id)
                    .OrderBy(x => x.SequenceNumber)
                    .ToList();

                list.Add(new TripShipment
                {
                    Id = Guid.NewGuid(),
                    TripId = trips[i].Id,
                    ShipmentId = shipments[i].Id,
                    ShipmentLegId = i < shipmentLegs.Count ? shipmentLegs[i].Id : null,
                    PickupTripStopId = stopsForTrip.Count > 0 ? stopsForTrip[0].Id : null,
                    DeliveryTripStopId = stopsForTrip.Count > 1 ? stopsForTrip[1].Id : null,
                    Priority = i % 5 + 1,
                    Notes = "Assigned to road trip",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.TripShipments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
