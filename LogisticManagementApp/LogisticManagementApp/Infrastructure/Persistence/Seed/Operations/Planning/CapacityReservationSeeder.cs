using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning
{
    public static class CapacityReservationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CapacityReservations.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();

            var list = new List<CapacityReservation>();

            for (int i = 0; i < 15 && i < shipments.Count; i++)
            {
                list.Add(new CapacityReservation
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    ReservedWeightKg = 250m + (i * 10),
                    ReservedVolumeCbm = 2m + (i * 0.2m),
                    ReservedFromUtc = DateTime.UtcNow.AddDays(i),
                    ReservedToUtc = DateTime.UtcNow.AddDays(i + 2),
                    Notes = "Capacity reserved for shipment",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CapacityReservations.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
