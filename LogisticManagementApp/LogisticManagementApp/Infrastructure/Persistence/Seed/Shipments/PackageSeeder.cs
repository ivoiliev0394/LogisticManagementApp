using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class PackageSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Packages.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var packages = new List<Package>();

            for (int i = 0; i < shipments.Count; i++)
            {
                packages.Add(new Package
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    PackageNo = $"PKG-{20000 + i}",
                    PackageType = PackageType.Pallet,
                    WeightKg = 120m + (i * 10m),
                    LengthCm = 120m,
                    WidthCm = 80m,
                    HeightCm = 150m + i,
                    VolumeCbm = 1.44m + (i * 0.05m),
                    Notes = "Standard EUR pallet",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Packages.AddRangeAsync(packages);
            await db.SaveChangesAsync();
        }
    }
}
