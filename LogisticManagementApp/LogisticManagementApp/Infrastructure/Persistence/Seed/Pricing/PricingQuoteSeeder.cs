using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class PricingQuoteSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.PricingQuotes.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var quotes = new List<PricingQuote>();

            for (int i = 0; i < companies.Count; i++)
            {
                quotes.Add(new PricingQuote
                {
                    Id = Guid.NewGuid(),
                    CustomerCompanyId = companies[i].Id,
                    QuoteNumber = $"QT-{2000 + i}",
                    NetAmount = 500 + i * 50,
                    TaxAmount = 100,
                    TotalAmount = 600 + i * 50,
                    Currency = "EUR",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.PricingQuotes.AddRangeAsync(quotes);
            await db.SaveChangesAsync();
        }
    }
}
