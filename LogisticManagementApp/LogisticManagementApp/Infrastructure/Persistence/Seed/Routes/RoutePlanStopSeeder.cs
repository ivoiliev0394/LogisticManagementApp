using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Routes
{
    public static class RoutePlanStopSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RoutePlanStops.AnyAsync())
                return;

            var routePlans = await db.RoutePlans
                .OrderBy(rp => rp.PlanDateUtc)
                .Take(15)
                .ToListAsync();

            var routeStops = await db.RouteStops
                .OrderBy(rs => rs.RouteId)
                .ThenBy(rs => rs.SequenceNo)
                .ToListAsync();

            if (!routePlans.Any() || !routeStops.Any())
                return;

            var list = new List<RoutePlanStop>();

            foreach (var plan in routePlans)
            {
                var stops = routeStops
                    .Where(x => x.RouteId == plan.RouteId)
                    .OrderBy(x => x.SequenceNo)
                    .ToList();

                for (int i = 0; i < stops.Count; i++)
                {
                    var plannedArrival = plan.PlanDateUtc.Date.AddHours(8 + (i * 2));
                    var plannedDeparture = plannedArrival.AddHours(1);

                    list.Add(new RoutePlanStop
                    {
                        Id = Guid.NewGuid(),
                        RoutePlanId = plan.Id,
                        RouteStopId = stops[i].Id,
                        LocationId = stops[i].LocationId,
                        SequenceNo = stops[i].SequenceNo,
                        StopType = stops[i].StopType,
                        PlannedArrivalUtc = plannedArrival,
                        PlannedDepartureUtc = plannedDeparture,
                        ActualArrivalUtc = null,
                        ActualDepartureUtc = null,
                        Notes = "Planned stop",
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }

            await db.RoutePlanStops.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}