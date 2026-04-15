using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class TariffRateSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.TariffRates.AnyAsync())
                return;

            var tariffs = await db.Tariffs.ToListAsync();

            var rates = new List<TariffRate>();

            foreach (var tariff in tariffs)
            {
                rates.Add(new TariffRate { Id = Guid.NewGuid(), TariffId = tariff.Id, FromValue = 0, ToValue = 100, Price = 50, MinCharge = 30, StepValue = 1 });
                rates.Add(new TariffRate { Id = Guid.NewGuid(), TariffId = tariff.Id, FromValue = 100, ToValue = 500, Price = 120, MinCharge = 80, StepValue = 1 });
                rates.Add(new TariffRate { Id = Guid.NewGuid(), TariffId = tariff.Id, FromValue = 500, ToValue = 1000, Price = 250, MinCharge = 200, StepValue = 1 });
            }

            await db.TariffRates.AddRangeAsync(rates);
            await db.SaveChangesAsync();
        }
    }
}
