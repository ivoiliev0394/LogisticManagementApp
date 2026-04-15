using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;    

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Rail
{
    public static class RailMovementSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RailMovements.AnyAsync())
                return;

            var trains = await db.Trains.Take(15).ToListAsync();
            var railServices = await db.RailServices.Take(15).ToListAsync();
            var locations = await db.Locations.Take(15).ToListAsync();

            var movements = new List<RailMovement>();

            for (int i = 0; i < 15 && i < trains.Count && i < railServices.Count; i++)
            {
                movements.Add(new RailMovement
                {
                    Id = Guid.NewGuid(),
                    TrainId = trains[i].Id,
                    RailServiceId = railServices[i].Id,
                    MovementNo = $"RM-{6000 + i}",
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    Status = RailMovementStatus.Planned,
                    ScheduledDepartureUtc = DateTime.UtcNow.AddDays(i + 1),
                    ScheduledArrivalUtc = DateTime.UtcNow.AddDays(i + 3),
                    Notes = "Planned rail freight movement",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RailMovements.AddRangeAsync(movements);
            await db.SaveChangesAsync();
        }
    }
}
