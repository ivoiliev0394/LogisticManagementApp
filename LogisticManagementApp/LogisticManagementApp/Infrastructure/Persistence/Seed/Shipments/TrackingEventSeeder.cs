using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments
{
    public static class TrackingEventSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TrackingEvents.AnyAsync())
                return;

            var shipments = await db.Shipments.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var events = new List<TrackingEvent>();

            for (int i = 0; i < shipments.Count; i++)
            {
                events.Add(new TrackingEvent
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    EventType = TrackingEventType.PickedUp,
                    EventTimeUtc = DateTime.UtcNow.AddDays(-2),
                    LocationId = locations[i].Id,
                    Details = "Shipment picked up",
                    CreatedAtUtc = DateTime.UtcNow
                });

                events.Add(new TrackingEvent
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    EventType = TrackingEventType.InTransit,
                    EventTimeUtc = DateTime.UtcNow.AddDays(-1),
                    LocationId = locations[(i + 1) % locations.Count].Id,
                    Details = "Shipment in transit",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.TrackingEvents.AddRangeAsync(events);
            await db.SaveChangesAsync();
        }
    }
}
