using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Audit;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Audit
{
    public static class AuditLogSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.AuditLogs.AnyAsync())
                return;

            var logs = new List<AuditLog>();

            for (int i = 0; i < 15; i++)
            {
                logs.Add(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    ActionType = AuditActionType.Update,
                    EntityType = "Shipment",
                    EntityId = Guid.NewGuid(),
                    UserName = $"user-{i}",
                    OldValuesJson = "{ \"status\": \"Created\" }",
                    NewValuesJson = "{ \"status\": \"InTransit\" }",
                    ActionAtUtc = DateTime.UtcNow.AddMinutes(-i * 10),
                    IpAddress = $"192.168.1.{i + 1}",
                    Notes = "Seeded audit log",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.AuditLogs.AddRangeAsync(logs);
            await db.SaveChangesAsync();
        }
    }
}