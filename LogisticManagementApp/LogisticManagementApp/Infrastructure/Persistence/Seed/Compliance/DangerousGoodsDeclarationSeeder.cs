using LogisticManagementApp.Domain.Compliance;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Compliance
{
    public static class DangerousGoodsDeclarationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.DangerousGoodsDeclarations.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            if (!shipments.Any())
                return;

            var list = new List<DangerousGoodsDeclaration>();

            var types = Enum.GetValues<VesselType>();
            int type = 1;

            for (int i = 0; i < shipments.Count; i++)
            {
                list.Add(new DangerousGoodsDeclaration
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    UnNumber = $"UN120{i}",
                    ProperShippingName = "Flammable Liquid",
                    HazardClass = (HazardClass)type,
                    PackingGroup = PackingGroup.II,
                    NetQuantity = 100 + (i * 10),
                    QuantityUnit = "L",
                    HandlingInstructions = "Keep away from heat",
                    RequiresSpecialHandling = true,
                    Notes = "DG declaration",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.DangerousGoodsDeclarations.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}