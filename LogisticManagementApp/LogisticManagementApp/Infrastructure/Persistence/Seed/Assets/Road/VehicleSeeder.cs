using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road
{
    public static class VehicleSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Vehicles.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var vehicles = new List<Vehicle>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                vehicles.Add(new Vehicle
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    RegistrationNumber = $"CB{i + 1000}TX",
                    VehicleType = VehicleType.Truck,
                    Brand = i % 2 == 0 ? "Mercedes-Benz" : "Volvo",
                    Model = i % 2 == 0 ? "Actros" : "FH",
                    MaxWeightKg = 24000m,
                    MaxVolumeCbm = 92m,
                    IsActive = true,
                    Notes = "Long-haul road transport vehicle",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Vehicles.AddRangeAsync(vehicles);
            await db.SaveChangesAsync();
        }
    }
}
