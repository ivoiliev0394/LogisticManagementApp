using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Routes
{
    public static class RouteStopSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RouteStops.AnyAsync())
                return;

            var routes = await db.Routes
                .OrderBy(r => r.RouteCode)
                .Take(15)
                .ToListAsync();

            var locations = await db.Locations
                .OrderBy(l => l.Name)
                .Take(15)
                .ToListAsync();

            if (!routes.Any() || locations.Count < 2)
                return;

            var stops = new List<RouteStop>();

            for (int i = 0; i < routes.Count && i < locations.Count; i++)
            {
                stops.Add(new RouteStop
                {
                    Id = Guid.NewGuid(),
                    RouteId = routes[i].Id,
                    LocationId = locations[i].Id,
                    SequenceNo = 1,
                    StopType = RouteStopType.Pickup,
                    PlannedArrivalTime = new TimeSpan(8, 0, 0),
                    PlannedDepartureTime = new TimeSpan(9, 0, 0),
                    CreatedAtUtc = DateTime.UtcNow
                });

                stops.Add(new RouteStop
                {
                    Id = Guid.NewGuid(),
                    RouteId = routes[i].Id,
                    LocationId = locations[(i + 1) % locations.Count].Id,
                    SequenceNo = 2,
                    StopType = RouteStopType.Delivery,
                    PlannedArrivalTime = new TimeSpan(10, 30, 0),
                    PlannedDepartureTime = new TimeSpan(12, 0, 0),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RouteStops.AddRangeAsync(stops);
            await db.SaveChangesAsync();
        }
    }
}