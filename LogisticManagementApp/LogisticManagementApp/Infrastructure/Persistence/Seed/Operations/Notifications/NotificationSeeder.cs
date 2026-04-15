using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Notifications;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Notifications
{
    public static class NotificationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Notifications.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var notifications = new List<Notification>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = $"Shipment Update {i + 1}",
                    Message = $"Shipment SHP-{10000 + i} has changed status.",
                    NotificationType = NotificationType.ShipmentUpdate,
                    Channel = NotificationChannel.InApp,
                    RecipientCompanyId = companies[i].Id,
                    IsRead = i % 3 == 0,
                    ReadAtUtc = i % 3 == 0 ? DateTime.UtcNow.AddHours(-2) : null,
                    SentAtUtc = DateTime.UtcNow.AddHours(-1),
                    RelatedEntityType = "Shipment",
                    RelatedEntityId = null,
                    Notes = "System-generated notification",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Notifications.AddRangeAsync(notifications);
            await db.SaveChangesAsync();
        }
    }
}
