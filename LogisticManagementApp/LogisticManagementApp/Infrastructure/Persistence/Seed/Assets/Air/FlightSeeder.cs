using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class FlightSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Flights.AnyAsync())
                return;

            var aircraft = await db.Aircraft.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var flights = new List<Flight>();

            for (int i = 0; i < 15 && i < aircraft.Count; i++)
            {
                flights.Add(new Flight
                {
                    Id = Guid.NewGuid(),
                    AircraftId = aircraft[i].Id,
                    FlightNumber = $"BGC{100 + i}",
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    ScheduledDepartureUtc = DateTime.UtcNow.AddDays(i + 1),
                    ScheduledArrivalUtc = DateTime.UtcNow.AddDays(i + 1).AddHours(4),
                    Notes = "Scheduled cargo flight",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Flights.AddRangeAsync(flights);
            await db.SaveChangesAsync();
        }
    }
}
