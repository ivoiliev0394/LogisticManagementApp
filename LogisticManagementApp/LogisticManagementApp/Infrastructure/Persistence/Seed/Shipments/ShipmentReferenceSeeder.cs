using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentReferenceSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentReferences.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var refs = new List<ShipmentReference>();

            for (int i = 0; i < shipments.Count; i++)
            {
                refs.Add(new ShipmentReference
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    ReferenceType = ShipmentReferenceType.InternalReference,
                    ReferenceValue = $"CUST-{6000 + i}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                refs.Add(new ShipmentReference
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    ReferenceType = ShipmentReferenceType.InternalReference,
                    ReferenceValue = $"INT-{9000 + i}",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentReferences.AddRangeAsync(refs);
            await db.SaveChangesAsync();
        }
    }
}
