using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations
{
    public static class ConsolidationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Consolidations.AnyAsync())
                return;

            var list = new List<Consolidation>();

            for (int i = 0; i < 15; i++)
            {
                list.Add(new Consolidation
                {
                    Id = Guid.NewGuid(),
                    ConsolidationNo = $"CON-{7000 + i}",
                    PlannedDepartureUtc = DateTime.UtcNow.AddDays(i + 1),
                    PlannedArrivalUtc = DateTime.UtcNow.AddDays(i + 3),
                    MasterReference = $"MREF-{9000 + i}",
                    Notes = "Group of shipments consolidated",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Consolidations.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}