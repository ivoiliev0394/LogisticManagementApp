using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Orders
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Orders.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            var addresses = await db.Addresses
                .OrderBy(a => a.City)
                .Take(15)
                .ToListAsync();

            var clients = await db.ClientProfiles
                .OrderBy(c => c.FirstName)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var list = new List<Order>();

            for (int i = 0; i < companies.Count; i++)
            {
                var company = companies[i];
                var client = i < clients.Count ? clients[i] : null;

                list.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNo = $"ORD-2026-{1000 + i}",
                    CustomerCompanyId = companies[i].Id,
                    ClientProfileId = client?.Id,
                    PickupAddressId = i < addresses.Count ? addresses[i].Id : null,
                    DeliveryAddressId = i < addresses.Count ? addresses[(i + 1) % addresses.Count].Id : null,
                    Status = OrderStatus.Draft,
                    Priority = i % 2 == 0 ? OrderPriority.Normal : OrderPriority.High,
                    RequestedPickupDateUtc = DateTime.UtcNow.AddDays(i),
                    CustomerReference = $"PO-{5000 + i}",
                    Notes = $"Seeded order for {company.Name} - {client.User.UserName}",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Orders.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}