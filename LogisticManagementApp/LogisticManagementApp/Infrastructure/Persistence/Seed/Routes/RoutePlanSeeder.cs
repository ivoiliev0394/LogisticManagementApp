using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Routes
{
    public static class RoutePlanSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RoutePlans.AnyAsync())
                return;

            var routes = await db.Routes
                .OrderBy(r => r.RouteCode)
                .Take(15)
                .ToListAsync();

            if (!routes.Any())
                return;

            var plans = new List<RoutePlan>();

            for (int i = 0; i < routes.Count; i++)
            {
                plans.Add(new RoutePlan
                {
                    Id = Guid.NewGuid(),
                    RouteId = routes[i].Id,
                    PlanDateUtc = DateTime.UtcNow.Date.AddDays(i),
                    Notes = "Daily route execution plan",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RoutePlans.AddRangeAsync(plans);
            await db.SaveChangesAsync();
        }
    }
}