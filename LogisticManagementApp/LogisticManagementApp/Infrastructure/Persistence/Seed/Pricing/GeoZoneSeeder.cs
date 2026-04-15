using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class GeoZoneSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.GeoZones.AnyAsync())
                return;

            var zones = new List<GeoZone>
        {
            new() { Id = Guid.NewGuid(), Code = "Z-A", Name = "Local Zone", Description = "Domestic shipments", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-B", Name = "Regional Zone", Description = "Neighbor countries", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-C", Name = "EU Zone", Description = "European Union", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-D", Name = "International Zone", Description = "Worldwide", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-E", Name = "Express Corridor", Description = "Priority routes", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-F", Name = "Sea Routes", Description = "Ocean transport zones", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-G", Name = "Air Routes", Description = "Air corridors", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-H", Name = "Rail Network", Description = "Rail corridors", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-I", Name = "Balkan Region", Description = "Balkan countries", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-J", Name = "Middle East", Description = "Middle East zone", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-K", Name = "Asia Pacific", Description = "Asia region", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-L", Name = "North America", Description = "USA/Canada", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-M", Name = "South America", Description = "LATAM", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-N", Name = "Africa", Description = "African region", CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "Z-O", Name = "Special Logistics", Description = "Special handling", CreatedAtUtc = DateTime.UtcNow }
        };

            await db.GeoZones.AddRangeAsync(zones);
            await db.SaveChangesAsync();
        }
    }
}
