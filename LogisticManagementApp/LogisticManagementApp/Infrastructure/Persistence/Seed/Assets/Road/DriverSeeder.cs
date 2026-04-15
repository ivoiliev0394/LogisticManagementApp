using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road
{
    public static class DriverSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Drivers.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var drivers = new List<Driver>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                drivers.Add(new Driver
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    FullName = $"Driver {i + 1} Petrov",
                    LicenseNumber = $"LIC-{7000 + i}",
                    Phone = $"+35988955{i:000}",
                    IsActive = true,
                    Notes = "International road transport driver",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Drivers.AddRangeAsync(drivers);
            await db.SaveChangesAsync();
        }
    }
}
