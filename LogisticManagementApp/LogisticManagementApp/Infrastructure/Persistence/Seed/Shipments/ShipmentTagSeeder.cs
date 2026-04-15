using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentTagSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentTags.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var types = Enum.GetValues<ShipmentTagType>();
            int type = 1;

            var tags = new List<ShipmentTag>();

            for (int i = 0; i < shipments.Count; i++)
            {
                tags.Add(new ShipmentTag
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    TagType = (ShipmentTagType)type,
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length) type++;
                else type = 1;
            }

            await db.ShipmentTags.AddRangeAsync(tags);
            await db.SaveChangesAsync();
        }
    }
}
