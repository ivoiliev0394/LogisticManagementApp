using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class VesselPositionSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.VesselPositions.AnyAsync())
                return;

            var vessels = await db.Vessels
                .OrderBy(v => v.Name)
                .Take(15)
                .ToListAsync();

            var positions = new List<VesselPosition>();

            for (int i = 0; i < vessels.Count; i++)
            {
                positions.Add(new VesselPosition
                {
                    Id = Guid.NewGuid(),
                    VesselId = vessels[i].Id,
                    Latitude = 42.7m + (i * 0.1m),
                    Longitude = 27.7m + (i * 0.1m),
                    SpeedKnots = 18 + i,
                    CourseDegrees = 90 + i,
                    PositionTimeUtc = DateTime.UtcNow.AddHours(-i),
                    Source = "Seed",
                    Notes = "Seeded vessel position"
                });
            }

            await db.VesselPositions.AddRangeAsync(positions);
            await db.SaveChangesAsync();
        }
    }
}
