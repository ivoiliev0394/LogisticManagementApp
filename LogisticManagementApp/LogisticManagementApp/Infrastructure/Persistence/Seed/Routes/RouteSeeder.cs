using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;
using Route = LogisticManagementApp.Domain.Routes.Route;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Routes
{
    public static class RouteSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Routes.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var routes = new List<Route>();

            for (int i = 0; i < companies.Count; i++)
            {
                routes.Add(new Route
                {
                    Id = Guid.NewGuid(),
                    RouteCode = $"R-{100 + i}",
                    CompanyId = companies[i].Id,
                    Name = $"Route {i + 1}",
                    Notes = "Regional delivery route",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Routes.AddRangeAsync(routes);
            await db.SaveChangesAsync();
        }
    }
}