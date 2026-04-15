using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class PaymentAllocationSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.PaymentAllocations.AnyAsync())
                return;

            var payments = await db.Payments.Take(15).ToListAsync();

            var allocations = new List<PaymentAllocation>();

            foreach (var payment in payments)
            {
                allocations.Add(new PaymentAllocation
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    InvoiceId = payment.InvoiceId,
                    AllocatedAmount = payment.Amount,
                    AllocatedAtUtc = payment.PaymentDateUtc,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.PaymentAllocations.AddRangeAsync(allocations);
            await db.SaveChangesAsync();
        }
    }
}
