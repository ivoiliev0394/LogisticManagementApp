using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Orders
{
    public static class OrderLineSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.OrderLines.AnyAsync())
                return;

            var orders = await db.Orders
                .OrderBy(o => o.OrderNo)
                .Take(15)
                .ToListAsync();

            if (!orders.Any())
                return;

            var list = new List<OrderLine>();

            foreach (var order in orders)
            {
                for (int i = 1; i <= 2; i++)
                {
                    list.Add(new OrderLine
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        LineNo = i,
                        Description = $"Goods item {i}",
                        Quantity = 10 + i,
                        QuantityUnit = "pcs",
                        GrossWeightKg = 100 + (i * 20),
                        VolumeCbm = 1.5m + (i * 0.5m),
                        IsDangerousGoods = false,
                        HsCode = "1234.56",
                        OriginCountry = "BG",
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }

            await db.OrderLines.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}