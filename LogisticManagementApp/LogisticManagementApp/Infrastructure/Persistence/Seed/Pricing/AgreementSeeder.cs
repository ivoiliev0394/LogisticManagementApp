using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class AgreementSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Agreements.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var agreements = new List<Agreement>();

            for (int i = 0; i < companies.Count; i++)
            {
                agreements.Add(new Agreement
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    AgreementNumber = $"AGR-{1000 + i}",
                    ValidFromUtc = DateTime.UtcNow.AddMonths(-3),
                    ValidToUtc = DateTime.UtcNow.AddYears(1),
                    Currency = "EUR",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.Agreements.AddRangeAsync(agreements);
            await db.SaveChangesAsync();
        }
    }
}
