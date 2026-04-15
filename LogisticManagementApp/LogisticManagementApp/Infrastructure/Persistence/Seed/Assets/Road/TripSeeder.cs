using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road
{
    public static class TripSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Trips.AnyAsync())
                return;

            var vehicles = await db.Vehicles.Take(15).ToListAsync();
            var drivers = await db.Drivers.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var trips = new List<Trip>();

            for (int i = 0; i < 15 && i < vehicles.Count && i < drivers.Count && i < locations.Count; i++)
            {
                trips.Add(new Trip
                {
                    Id = Guid.NewGuid(),
                    TripNo = $"TRP-{4000 + i}",
                    VehicleId = vehicles[i].Id,
                    DriverId = drivers[i].Id,
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    Status = TripStatus.Planned,
                    PlannedDepartureUtc = DateTime.UtcNow.AddDays(i + 1),
                    PlannedArrivalUtc = DateTime.UtcNow.AddDays(i + 2),
                    Notes = "Scheduled regional transport trip",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Trips.AddRangeAsync(trips);
            await db.SaveChangesAsync();
        }
    }
}
