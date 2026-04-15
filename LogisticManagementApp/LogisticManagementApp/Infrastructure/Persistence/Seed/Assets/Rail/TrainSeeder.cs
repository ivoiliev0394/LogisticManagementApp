using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Rail
{
    public static class TrainSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Trains.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var trains = new List<Train>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                trains.Add(new Train
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    TrainNumber = $"TR-{900 + i}",
                    MaxWeightKg = 200000m,
                    MaxVolumeCbm = 1000m,
                    IsActive = true,
                    Notes = "Freight train",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Trains.AddRangeAsync(trains);
            await db.SaveChangesAsync();
        }
    }
}
