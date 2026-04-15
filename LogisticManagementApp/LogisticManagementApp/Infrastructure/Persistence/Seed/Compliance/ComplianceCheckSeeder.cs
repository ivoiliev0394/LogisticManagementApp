using LogisticManagementApp.Domain.Compliance;
using LogisticManagementApp.Domain.Enums.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Compliance
{
    public static class ComplianceCheckSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ComplianceChecks.AnyAsync())
                return;

            var shipments = await db.Shipments
                .OrderBy(s => s.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            if (!shipments.Any())
                return;

            var list = new List<ComplianceCheck>();

            for (int i = 0; i < shipments.Count; i++)
            {
                list.Add(new ComplianceCheck
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    CheckType = "Customs",
                    Status = i % 2 == 0
                        ? ComplianceCheckStatus.Passed
                        : ComplianceCheckStatus.Pending,
                    CheckedAtUtc = DateTime.UtcNow.AddHours(-i),
                    CheckedBy = $"user-{i}",
                    ResultDetails = "OK",
                    Notes = "Compliance check performed",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.ComplianceChecks.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}