using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Enums;
using LogisticManagementApp.Domain.Enums.Billing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Billing;

public static class ChargeSeeder
{
    public static async Task SeedAsync(LogisticAppDbContext db)
    {
        if (await db.Charges.AnyAsync())
            return;

        var shipments = await db.Shipments
            .Take(15)
            .ToListAsync();

        if (shipments.Count == 0)
            return;

        var charges = new List<Charge>();

        for (int i = 0; i < shipments.Count; i++)
        {
            charges.Add(new Charge
            {
                Id = Guid.NewGuid(),
                ShipmentId = shipments[i].Id,
                ShipmentLegId = null,
                ChargeCode = $"FRT-{1000 + i}",
                Description = "Base Transport Fee",
                Quantity = 1m,
                UnitPrice = 400m + (i * 10),
                Currency = "EUR",
                SourceType = ChargeSourceType.Manual,
                IsTaxable = true,
                TaxRatePercent = 20m,
                Notes = "Seeded base freight charge",
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await db.Charges.AddRangeAsync(charges);
        await db.SaveChangesAsync();
    }
}