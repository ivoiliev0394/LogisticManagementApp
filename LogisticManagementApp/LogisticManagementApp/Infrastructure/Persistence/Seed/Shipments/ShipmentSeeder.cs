using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Shipments.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();
            var orders = await db.Orders.Take(15).ToListAsync();
            var addresses = await db.Addresses.Take(15).ToListAsync();

            var shipments = new List<Shipment>();

            var types = Enum.GetValues<TransportMode>();
            int type = 1;

            for (int i = 0; i < 15; i++)
            {
                shipments.Add(new Shipment
                {
                    Id = Guid.NewGuid(),
                    ShipmentNo = $"SHP-{10000 + i}",
                    CustomerCompanyId = companies[i].Id,
                    OrderId = i < orders.Count ? orders[i].Id : null,
                    SenderAddressId = addresses[i].Id,
                    ReceiverAddressId = addresses[(i + 1) % addresses.Count].Id,
                    Status = ShipmentStatus.Created,
                    PrimaryMode = (TransportMode)type,
                    Incoterm = Incoterm.EXW,
                    DeclaredValue = 1500m + (i * 250m),
                    Currency = "EUR",
                    CustomerReference = $"REF-{5000 + i}",
                    Notes = $"Shipment for {companies[i].Name}",
                    CreatedAtUtc = DateTime.UtcNow
                });
                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.Shipments.AddRangeAsync(shipments);
            await db.SaveChangesAsync();
        }
    }
}
