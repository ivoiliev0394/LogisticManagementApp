using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class AircraftSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Aircraft.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var aircraft = new List<Aircraft>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                aircraft.Add(new Aircraft
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    TailNumber = $"LZ-CG{i:000}",
                    Manufacturer = i % 2 == 0 ? "Boeing" : "Airbus",
                    Model = i % 2 == 0 ? "747-8F" : "A330-200F",
                    MaxPayloadKg = 65000m + (i * 1000m),
                    MaxVolumeCbm = 700m + (i * 10m),
                    IsActive = true,
                    Notes = "Cargo aircraft",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Aircraft.AddRangeAsync(aircraft);
            await db.SaveChangesAsync();
        }
    }
}
