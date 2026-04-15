using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class CreditNoteSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CreditNotes.AnyAsync())
                return;

            var invoices = await db.Invoices.Take(15).ToListAsync();

            var statuses = Enum.GetValues<CreditNoteStatus>(); 
            int status = 1;

            var notes = new List<CreditNote>();

            for (int i = 0; i < invoices.Count; i++)
            {
                notes.Add(new CreditNote
                {
                    Id = Guid.NewGuid(),
                    CreditNoteNo = $"CRN-{7000 + i}",
                    InvoiceId = invoices[i].Id,
                    BillToCompanyId = invoices[i].BillToCompanyId,
                    IssueDateUtc = DateTime.UtcNow.Date.AddDays(-2 + i),
                    Currency = "EUR",
                    Status = (CreditNoteStatus)status,
                    NetAmount = 20m + i,
                    TaxAmount = 4m + (i * 0.2m),
                    TotalAmount = 24m + (i * 1.2m),
                    Reason = "Commercial discount adjustment",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (status < statuses.Length) status++; 
                else status = 1;
            }

            await db.CreditNotes.AddRangeAsync(notes);
            await db.SaveChangesAsync();
        }
    }
}
