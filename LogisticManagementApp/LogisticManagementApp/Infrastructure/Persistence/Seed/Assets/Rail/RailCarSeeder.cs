using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Rail
{
    public static class RailCarSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.RailCars.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var cars = new List<RailCar>();

            for (int i = 0; i < 15; i++)
            {
                var company = companies[i % companies.Count];

                cars.Add(new RailCar
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    RailCarNumber = $"RC-{10000 + i}",
                    MaxWeightKg = 60000m,
                    MaxVolumeCbm = 120m,
                    Status = AssetStatus.Available,
                    IsActive = true,
                    Notes = "Seeded rail car",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.RailCars.AddRangeAsync(cars);
            await db.SaveChangesAsync();
        }
    }
}
