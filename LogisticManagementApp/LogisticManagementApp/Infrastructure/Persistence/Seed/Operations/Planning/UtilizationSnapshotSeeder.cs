using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning
{
    public static class UtilizationSnapshotSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.UtilizationSnapshots.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var list = new List<UtilizationSnapshot>();

            var types = Enum.GetValues<VesselType>();
            int type = 1;

            for (int i = 0; i < companies.Count; i++)
            {
                var total = 1000m;
                var used = 400m + (i * 20);
                var free = total - used;

                list.Add(new UtilizationSnapshot
                {
                    Id = Guid.NewGuid(),
                    ResourceType = (ResourceType)type, // смени, ако enum-ът ти няма Company
                    ResourceId = companies[i].Id,
                    SnapshotDateUtc = DateTime.UtcNow.Date.AddDays(-i),
                    TotalCapacity = total,
                    UsedCapacity = used,
                    FreeCapacity = free,
                    UtilizationPercent = total == 0 ? 0 : (used / total) * 100,
                    Notes = "Seeded utilization snapshot",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.UtilizationSnapshots.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}