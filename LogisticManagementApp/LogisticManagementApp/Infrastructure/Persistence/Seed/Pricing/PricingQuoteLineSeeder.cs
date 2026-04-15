using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class PricingQuoteLineSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.PricingQuoteLines.AnyAsync())
                return;

            var quotes = await db.PricingQuotes.ToListAsync();

            var lines = new List<PricingQuoteLine>();

            foreach (var quote in quotes)
            {
                lines.Add(new PricingQuoteLine
                {
                    Id = Guid.NewGuid(),
                    PricingQuoteId = quote.Id,
                    Description = "Transport Charge",
                    Quantity = 1,
                    UnitPrice = 300,
                    LineAmount = 300,
                    CreatedAtUtc = DateTime.UtcNow
                });

                lines.Add(new PricingQuoteLine
                {
                    Id = Guid.NewGuid(),
                    PricingQuoteId = quote.Id,
                    Description = "Handling Fee",
                    Quantity = 1,
                    UnitPrice = 200,
                    LineAmount = 200,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.PricingQuoteLines.AddRangeAsync(lines);
            await db.SaveChangesAsync();
        }
    }
}
