using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Notifications;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Notifications
{
    public static class NotificationSubscriptionSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.NotificationSubscriptions.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var subscriptions = new List<NotificationSubscription>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                subscriptions.Add(new NotificationSubscription
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    EventKey = i % 2 == 0 ? "ShipmentStatusChanged" : "InvoiceOverdue",
                    Channel = i % 2 == 0 ? NotificationChannel.InApp : NotificationChannel.Email,
                    IsEnabled = true,
                    Notes = "Default company subscription",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.NotificationSubscriptions.AddRangeAsync(subscriptions);
            await db.SaveChangesAsync();
        }
    }
}
