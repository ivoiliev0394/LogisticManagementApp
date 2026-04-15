using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Orders
{
    public static class OrderStatusHistorySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.OrderStatusHistories.AnyAsync())
                return;

            var orders = await db.Orders
                .OrderBy(o => o.CreatedAtUtc)
                .Take(15)
                .ToListAsync();

            if (!orders.Any())
                return;

            var list = new List<OrderStatusHistory>();

            foreach (var order in orders)
            {
                list.Add(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OldStatus = OrderStatus.Draft,
                    NewStatus = OrderStatus.Confirmed,
                    ChangedAtUtc = DateTime.UtcNow,
                    Reason = "Order confirmed",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.OrderStatusHistories.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}