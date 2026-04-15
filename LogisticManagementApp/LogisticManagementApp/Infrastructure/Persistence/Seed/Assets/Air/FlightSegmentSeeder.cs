using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class FlightSegmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.FlightSegments.AnyAsync())
                return;

            var flights = await db.Flights.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var segments = new List<FlightSegment>();

            for (int i = 0; i < 15 && i < flights.Count; i++)
            {
                segments.Add(new FlightSegment
                {
                    Id = Guid.NewGuid(),
                    FlightId = flights[i].Id,
                    SegmentNo = 1,
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    ScheduledDepartureUtc = flights[i].ScheduledDepartureUtc,
                    ScheduledArrivalUtc = flights[i].ScheduledArrivalUtc,
                    Notes = "Main flight segment",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.FlightSegments.AddRangeAsync(segments);
            await db.SaveChangesAsync();
        }
    }
}
