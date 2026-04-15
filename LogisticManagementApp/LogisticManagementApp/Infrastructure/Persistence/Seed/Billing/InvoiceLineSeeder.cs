using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class InvoiceLineSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.InvoiceLines.AnyAsync())
                return;

            var invoices = await db.Invoices.ToListAsync();
            var charges = await db.Charges.ToListAsync();

            var lines = new List<InvoiceLine>();

            for (int i = 0; i < invoices.Count; i++)
            {
                lines.Add(new InvoiceLine
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoices[i].Id,
                    ChargeId = i < charges.Count ? charges[i].Id : null,
                    LineNo = 1,
                    Description = "Transport Charge",
                    Quantity = 1m,
                    UnitPrice = 300m + (i * 10),
                    TaxRatePercent = 20m,
                    LineNetAmount = 300m + (i * 10),
                    LineTaxAmount = 60m + (i * 2),
                    LineTotalAmount = 360m + (i * 12),
                    CreatedAtUtc = DateTime.UtcNow
                });

                lines.Add(new InvoiceLine
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoices[i].Id,
                    ChargeId = null,
                    LineNo = 2,
                    Description = "Handling Fee",
                    Quantity = 1m,
                    UnitPrice = 100m + (i * 5),
                    TaxRatePercent = 20m,
                    LineNetAmount = 100m + (i * 5),
                    LineTaxAmount = 20m + i,
                    LineTotalAmount = 120m + (i * 6),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.InvoiceLines.AddRangeAsync(lines);
            await db.SaveChangesAsync();
        }
    }
}
