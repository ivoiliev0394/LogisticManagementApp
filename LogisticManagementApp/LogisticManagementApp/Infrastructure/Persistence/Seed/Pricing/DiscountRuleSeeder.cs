using LogisticManagementApp.Domain.Enums.Pricing;
using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class DiscountRuleSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.DiscountRules.AnyAsync())
                return;

            var agreements = await db.Agreements.Take(15).ToListAsync();

            var rules = new List<DiscountRule>();

            for (int i = 0; i < agreements.Count; i++)
            {
                rules.Add(new DiscountRule
                {
                    Id = Guid.NewGuid(),
                    AgreementId = agreements[i].Id,
                    DiscountType = DiscountType.Percent,
                    Value = 5 + i % 10,
                    MinShipmentValue = 100,
                    MaxShipmentValue = 10000,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.DiscountRules.AddRangeAsync(rules);
            await db.SaveChangesAsync();
        }
    }
}
