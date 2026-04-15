using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Orders
{
    public static class OrderAttachmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.OrderAttachments.AnyAsync())
                return;

            var orders = await db.Orders.Take(15).ToListAsync();
            var files = await db.FileResources.Take(15).ToListAsync();

            if (!orders.Any() || !files.Any())
                return;

            var count = Math.Min(orders.Count, files.Count);
            var list = new List<OrderAttachment>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new OrderAttachment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[i].Id,
                    FileResourceId = files[i].Id,
                    AttachmentType = "Invoice",
                    Notes = "Attached document",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.OrderAttachments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}