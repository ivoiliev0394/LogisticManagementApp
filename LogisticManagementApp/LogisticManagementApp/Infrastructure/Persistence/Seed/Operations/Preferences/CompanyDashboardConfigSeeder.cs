using LogisticManagementApp.Domain.Operations.Preferences;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Preferences
{
    public static class CompanyDashboardConfigSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CompanyDashboardConfigs.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var configs = new List<CompanyDashboardConfig>();

            for (int i = 0; i < companies.Count; i++)
            {
                configs.Add(new CompanyDashboardConfig
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    DashboardKey = "Main",
                    LayoutJson = "{ \"widgets\": [\"shipments\", \"invoices\", \"alerts\"] }",
                    WidgetSettingsJson = "{ \"theme\": \"" + (i % 2 == 0 ? "Light" : "Dark") + "\" }",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CompanyDashboardConfigs.AddRangeAsync(configs);
            await db.SaveChangesAsync();
        }
    }
}