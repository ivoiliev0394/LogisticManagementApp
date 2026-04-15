using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing
{
    public static class TaxRateSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TaxRates.AnyAsync())
                return;

            var rates = new List<TaxRate>
        {
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "BG VAT Standard", CountryCode = "BG", RatePercent = 20m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "RO VAT Standard", CountryCode = "RO", RatePercent = 19m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "GR VAT Standard", CountryCode = "GR", RatePercent = 24m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "DE VAT Standard", CountryCode = "DE", RatePercent = 19m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "NL VAT Standard", CountryCode = "NL", RatePercent = 21m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "FR VAT Standard", CountryCode = "FR", RatePercent = 20m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "IT VAT Standard", CountryCode = "IT", RatePercent = 22m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "AT VAT Standard", CountryCode = "AT", RatePercent = 20m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Vat, Name = "HU VAT Standard", CountryCode = "HU", RatePercent = 27m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Customs, Name = "Import Customs EU", CountryCode = "EU", RatePercent = 5m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Customs, Name = "Import Customs Turkey", CountryCode = "TR", RatePercent = 8m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Customs, Name = "Import Customs UK", CountryCode = "UK", RatePercent = 6m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.LocalTax, Name = "Port Local Fee", CountryCode = "BG", RatePercent = 2.5m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.LocalTax, Name = "Airport Local Fee", CountryCode = "GR", RatePercent = 1.8m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TaxType = TaxType.Other, Name = "Environmental Charge", CountryCode = "EU", RatePercent = 1.2m, ValidFromUtc = new DateTime(2024, 1, 1), IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        };

            await db.TaxRates.AddRangeAsync(rates);
            await db.SaveChangesAsync();
        }
    }
}
