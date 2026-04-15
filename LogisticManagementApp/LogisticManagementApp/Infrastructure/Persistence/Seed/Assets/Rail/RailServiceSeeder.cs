using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Rail
{
    public static class RailServiceSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RailServices.AnyAsync())
                return;

            var locations = await db.Locations
                .OrderBy(l => l.Name)
                .Take(15)
                .ToListAsync();

            if (locations.Count < 2)
                return;

            var services = new List<RailService>();

            for (int i = 0; i < Math.Min(15, locations.Count); i++)
            {
                var origin = locations[i];
                var destination = locations[(i + 1) % locations.Count];

                services.Add(new RailService
                {
                    Id = Guid.NewGuid(),
                    ServiceCode = $"RS-{3000 + i}",
                    Name = $"Rail Freight Service {i + 1}",
                    OriginLocationId = origin.Id,
                    DestinationLocationId = destination.Id,
                    EstimatedTransitDays = 2 + (i % 3),
                    IsActive = true,
                    Notes = "Scheduled rail freight service",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RailServices.AddRangeAsync(services);
            await db.SaveChangesAsync();
        }
    }
}
