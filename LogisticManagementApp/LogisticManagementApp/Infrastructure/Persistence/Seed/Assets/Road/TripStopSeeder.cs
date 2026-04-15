using LogisticManagementApp.Domain.Assets.Road;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road
{
    public static class TripStopSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TripStops.AnyAsync())
                return;

            var trips = await db.Trips.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var stops = new List<TripStop>();

            for (int i = 0; i < 15 && i < trips.Count; i++)
            {
                stops.Add(new TripStop
                {
                    Id = Guid.NewGuid(),
                    TripId = trips[i].Id,
                    LocationId = locations[i].Id,
                    SequenceNumber = 1,
                    PlannedArrivalUtc = trips[i].PlannedDepartureUtc,
                    PlannedDepartureUtc = trips[i].PlannedDepartureUtc?.AddHours(1),
                    Notes = "Pickup point",
                    CreatedAtUtc = DateTime.UtcNow
                });

                stops.Add(new TripStop
                {
                    Id = Guid.NewGuid(),
                    TripId = trips[i].Id,
                    LocationId = locations[(i + 1) % locations.Count].Id,
                    SequenceNumber = 2,
                    PlannedArrivalUtc = trips[i].PlannedArrivalUtc,
                    PlannedDepartureUtc = trips[i].PlannedArrivalUtc?.AddHours(1),
                    Notes = "Delivery point",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.TripStops.AddRangeAsync(stops);
            await db.SaveChangesAsync();
        }
    }
}
