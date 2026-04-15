using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning
{
    public static class AssignmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Assignments.AnyAsync())
                return;

            var shipmentLegs = await db.ShipmentLegs
                .OrderBy(sl => sl.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            var vehicles = await db.Vehicles
                .OrderBy(v => v.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            if (!shipmentLegs.Any() || !vehicles.Any())
                return;

            var count = Math.Min(shipmentLegs.Count, vehicles.Count);
            var list = new List<Assignment>();

            var types = Enum.GetValues<VesselType>();
            int type = 1;

            for (int i = 0; i < count; i++)
            {
                list.Add(new Assignment
                {
                    Id = Guid.NewGuid(),
                    ShipmentLegId = shipmentLegs[i].Id,
                    ResourceType = (ResourceType)type,
                    ResourceId = vehicles[i].Id,
                    Status = AssignmentStatus.Assigned,
                    AssignedAtUtc = DateTime.UtcNow.AddHours(-2),
                    ReferenceNo = $"ASN-{1000 + i}",
                    Notes = "Shipment leg assigned to vehicle",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.Assignments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}