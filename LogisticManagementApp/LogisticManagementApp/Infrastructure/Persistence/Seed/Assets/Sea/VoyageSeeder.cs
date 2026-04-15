using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class VoyageSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Voyages.AnyAsync())
                return;

            var vessels = await db.Vessels.Take(15).ToListAsync();

            var voyages = new List<Voyage>();

            for (int i = 0; i < vessels.Count; i++)
            {
                voyages.Add(new Voyage
                {
                    Id = Guid.NewGuid(),
                    VesselId = vessels[i].Id,
                    VoyageNumber = $"VOY-{100 + i}",
                    PlannedDepartureUtc = DateTime.UtcNow.AddDays(-10 + i),
                    PlannedArrivalUtc = DateTime.UtcNow.AddDays(10 + i),
                    OriginPort = "Varna",
                    DestinationPort = "Hamburg",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Voyages.AddRangeAsync(voyages);
            await db.SaveChangesAsync();
        }
    }
}
