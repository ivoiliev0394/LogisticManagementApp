using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class VoyageStopSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.VoyageStops.AnyAsync())
                return;

            var voyages = await db.Voyages
                .OrderBy(v => v.PlannedDepartureUtc)
                .Take(15)
                .ToListAsync();

            var varnaPort = await db.Locations.FirstOrDefaultAsync(l => l.Code == "VAR-PORT");
            var istanbulPort = await db.Locations.FirstOrDefaultAsync(l => l.Code == "IST-PORT");
            var hamburgPort = await db.Locations.FirstOrDefaultAsync(l => l.Code == "HAM-PORT");

            if (varnaPort == null || istanbulPort == null || hamburgPort == null)
                return;

            var stops = new List<VoyageStop>();

            foreach (var voyage in voyages)
            {
                var departure = voyage.PlannedDepartureUtc ?? DateTime.UtcNow;
                var arrival = voyage.PlannedArrivalUtc ?? departure.AddDays(5);

                stops.Add(new VoyageStop
                {
                    Id = Guid.NewGuid(),
                    VoyageId = voyage.Id,
                    LocationId = varnaPort.Id,
                    SequenceNumber = 1,
                    PlannedArrivalUtc = departure,
                    PlannedDepartureUtc = departure.AddHours(6),
                    CreatedAtUtc = DateTime.UtcNow
                });

                stops.Add(new VoyageStop
                {
                    Id = Guid.NewGuid(),
                    VoyageId = voyage.Id,
                    LocationId = istanbulPort.Id,
                    SequenceNumber = 2,
                    PlannedArrivalUtc = departure.AddDays(2),
                    PlannedDepartureUtc = departure.AddDays(2).AddHours(8),
                    CreatedAtUtc = DateTime.UtcNow
                });

                stops.Add(new VoyageStop
                {
                    Id = Guid.NewGuid(),
                    VoyageId = voyage.Id,
                    LocationId = hamburgPort.Id,
                    SequenceNumber = 3,
                    PlannedArrivalUtc = arrival,
                    PlannedDepartureUtc = null,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.VoyageStops.AddRangeAsync(stops);
            await db.SaveChangesAsync();
        }
    }
}
