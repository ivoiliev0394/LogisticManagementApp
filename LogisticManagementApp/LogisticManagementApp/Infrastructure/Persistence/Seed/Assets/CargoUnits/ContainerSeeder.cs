using LogisticManagementApp.Domain.Assets.CargoUnits;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.CargoUnits
{
    public static class ContainerSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Containers.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var containers = new List<Container>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                containers.Add(new Container
                {
                    Id = Guid.NewGuid(),
                    OwnerCompanyId = companies[i].Id,
                    ContainerNumber = $"MSCU{100000 + i}",
                    TareWeightKg = 2250m,
                    MaxGrossWeightKg = 30480m,
                    VolumeCbm = 33.2m,
                    SealNumber = $"SEAL-{9000 + i}",
                    IsActive = true,
                    Notes = "Standard dry container",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Containers.AddRangeAsync(containers);
            await db.SaveChangesAsync();
        }
    }
}
