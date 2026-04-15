using LogisticManagementApp.Domain.Assets.CargoUnits;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.CargoUnits
{
    public static class ContainerSealSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ContainerSeals.AnyAsync())
                return;

            var containers = await db.Containers.Take(15).ToListAsync();

            var seals = new List<ContainerSeal>();

            for (int i = 0; i < 15 && i < containers.Count; i++)
            {
                seals.Add(new ContainerSeal
                {
                    Id = Guid.NewGuid(),
                    ContainerId = containers[i].Id,
                    SealNumber = $"CS-{15000 + i}",
                    AppliedAtUtc = DateTime.UtcNow.AddDays(-i),
                    AppliedBy = $"Operator {i + 1}",
                    IsActiveSeal = true,
                    Notes = "Security seal applied",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ContainerSeals.AddRangeAsync(seals);
            await db.SaveChangesAsync();
        }
    }
}
