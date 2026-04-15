using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class PaymentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Payments.AnyAsync())
                return;

            var invoices = await db.Invoices.Take(15).ToListAsync();
            var statuses = Enum.GetValues<PaymentStatus>();
            int status = 1;

            var payments = new List<Payment>();

            for (int i = 0; i < invoices.Count; i++)
            {
                payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoices[i].Id,
                    PaymentDateUtc = DateTime.UtcNow.Date.AddDays(-5 + i),
                    Amount = invoices[i].TotalAmount / 2,
                    Currency = "EUR",
                    Status = (PaymentStatus)status,
                    TransactionReference = $"TX-{5000 + i}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (status < statuses.Length) status++;
                else status = 1;
            }

            await db.Payments.AddRangeAsync(payments);
            await db.SaveChangesAsync();
        }
    }
}
