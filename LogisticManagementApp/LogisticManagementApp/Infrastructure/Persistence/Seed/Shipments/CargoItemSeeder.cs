using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class CargoItemSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CargoItems.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var items = new List<CargoItem>();

            for (int i = 0; i < shipments.Count; i++)
            {
                items.Add(new CargoItem
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    Description = $"Industrial spare parts lot {i + 1}",
                    CargoItemType = CargoItemType.GeneralCargo,
                    Quantity = 5 + i,
                    UnitOfMeasure = "crates",
                    GrossWeightKg = 300m + (i * 25m),
                    NetWeightKg = 270m + (i * 20m),
                    VolumeCbm = 2.5m + (i * 0.2m),
                    LengthCm = 200m,
                    WidthCm = 120m,
                    HeightCm = 110m,
                    HsCode = "848310",
                    OriginCountry = "Germany",
                    IsStackable = i % 2 == 0,
                    IsFragile = i % 3 == 0,
                    Notes = "Handle with care",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CargoItems.AddRangeAsync(items);
            await db.SaveChangesAsync();
        }
    }
}
