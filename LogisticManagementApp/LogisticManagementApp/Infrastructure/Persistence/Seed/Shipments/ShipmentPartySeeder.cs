using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentPartySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentParties.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var companies = await db.Companies.Take(15).ToListAsync();

            var parties = new List<ShipmentParty>();

            for (int i = 0; i < shipments.Count; i++)
            {
                parties.Add(new ShipmentParty
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    CompanyId = companies[i].Id,
                    Role = PartyRole.Shipper,
                    CreatedAtUtc = DateTime.UtcNow
                });

                parties.Add(new ShipmentParty
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    CompanyId = companies[(i + 1) % companies.Count].Id,
                    Role = PartyRole.Consignee,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentParties.AddRangeAsync(parties);
            await db.SaveChangesAsync();
        }
    }
}
