using LogisticManagementApp.Domain.Operations.Preferences;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Preferences
{
    public static class SavedFilterSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.SavedFilters.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var filters = new List<SavedFilter>();

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                filters.Add(new SavedFilter
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    Name = $"Filter {i + 1}",
                    EntityType = "Shipment",
                    FilterJson = "{ \"status\": \"InTransit\" }",
                    IsDefault = i == 0,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.SavedFilters.AddRangeAsync(filters);
            await db.SaveChangesAsync();
        }
    }
}
