using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class InvoiceSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Invoices.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();
            var statuses= Enum.GetValues<InvoiceStatus>();
            int status= 1;
            var invoices = new List<Invoice>();
            
            for (int i = 0; i < companies.Count; i++)
            {
                invoices.Add(new Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNo = $"INV-{3000 + i}",
                    BillToCompanyId = companies[i].Id,
                    IssueDateUtc = DateTime.UtcNow.Date.AddDays(-15 + i),
                    DueDateUtc = DateTime.UtcNow.Date.AddDays(15 + i),
                    Currency = "EUR",
                    Status = (InvoiceStatus)status,
                    SubtotalAmount = 400m + (i * 25),
                    TaxAmount = 80m + (i * 5),
                    TotalAmount = 480m + (i * 30),
                    CreatedAtUtc = DateTime.UtcNow
                });

                if(status<statuses.Length)status++;
                else status=1;
            }

            await db.Invoices.AddRangeAsync(invoices);
            await db.SaveChangesAsync();
        }
    }
}
