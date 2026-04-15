using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class ShipmentStatusHistorySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ShipmentStatusHistories.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var history = new List<ShipmentStatusHistory>();

            foreach (var shipment in shipments)
            {
                history.Add(new ShipmentStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipment.Id,
                    OldStatus = ShipmentStatus.Draft,
                    NewStatus = ShipmentStatus.Created,
                    ChangedAtUtc = DateTime.UtcNow.AddDays(-3),
                    Reason = "Shipment created",
                    CreatedAtUtc = DateTime.UtcNow
                });

                history.Add(new ShipmentStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipment.Id,
                    OldStatus = ShipmentStatus.PickedUp,
                    NewStatus = ShipmentStatus.InTransit,
                    ChangedAtUtc = DateTime.UtcNow.AddDays(-1),
                    Reason = "Shipment in transit",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ShipmentStatusHistories.AddRangeAsync(history);
            await db.SaveChangesAsync();
        }
    }
}
