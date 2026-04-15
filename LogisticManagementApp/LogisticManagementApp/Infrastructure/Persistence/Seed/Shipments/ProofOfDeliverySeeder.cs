using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ProofOfDeliverySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ProofOfDeliveries.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var files = await db.FileResources.Take(15).ToListAsync();

            var pods = new List<ProofOfDelivery>();

            for (int i = 0; i < shipments.Count && i < files.Count; i++)
            {
                pods.Add(new ProofOfDelivery
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    DeliveredAtUtc = DateTime.UtcNow.AddDays(-1),
                    ReceiverName = $"Receiver {i + 1}",
                    SignatureFileResourceId = files[i].Id,
                    Notes = "Delivered successfully",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ProofOfDeliveries.AddRangeAsync(pods);
            await db.SaveChangesAsync();
        }
    }
}
