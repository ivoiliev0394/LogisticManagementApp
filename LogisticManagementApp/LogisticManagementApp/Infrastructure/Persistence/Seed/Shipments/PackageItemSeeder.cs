using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class PackageItemSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.PackageItems.AnyAsync())
                return;

            var packages = await db.Packages.Take(15).ToListAsync();

            var items = new List<PackageItem>();

            for (int i = 0; i < packages.Count; i++)
            {
                items.Add(new PackageItem
                {
                    Id = Guid.NewGuid(),
                    PackageId = packages[i].Id,
                    Description = $"Consumer electronics batch {i + 1}",
                    Quantity = 10 + i,
                    Unit = "pcs",
                    HsCode = "847130",
                    OriginCountry = "China",
                    UnitPrice = 85m + i,
                    Currency = "EUR",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.PackageItems.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }
    }
}
