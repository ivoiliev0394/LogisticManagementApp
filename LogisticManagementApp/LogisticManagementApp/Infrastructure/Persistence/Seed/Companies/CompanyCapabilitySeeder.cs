using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Companies;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Companies
{
    public static class CompanyCapabilitySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CompanyCapabilities.AnyAsync())
                return;

            var companies = await db.Companies.OrderBy(x => x.Name).Take(15).ToListAsync();

            var capabilityTypes = new[]
            {
            CompanyCapabilityType.FreightForwarder,
            CompanyCapabilityType.SeaCarrier,
            CompanyCapabilityType.AirCarrier,
            CompanyCapabilityType.RoadCarrier,
            CompanyCapabilityType.WarehouseOperator,
            CompanyCapabilityType.TerminalOperator,
            CompanyCapabilityType.RailCarrier,
            CompanyCapabilityType.CustomsBroker,
            CompanyCapabilityType.CourierOperator,
            CompanyCapabilityType.SeaCarrier,
            CompanyCapabilityType.WarehouseOperator,
            CompanyCapabilityType.TerminalOperator,
            CompanyCapabilityType.AirCarrier,
            CompanyCapabilityType.RoadCarrier,
            CompanyCapabilityType.FreightForwarder
        };

            var capabilities = new List<CompanyCapability>();

            for (int i = 0; i < 15; i++)
            {
                capabilities.Add(new CompanyCapability
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    CapabilityType = capabilityTypes[i],
                    IsEnabled = true,
                    ValidFromUtc = DateTime.UtcNow.Date.AddMonths(-6),
                    ValidToUtc = DateTime.UtcNow.Date.AddYears(2),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CompanyCapabilities.AddRangeAsync(capabilities);
            await db.SaveChangesAsync();
        }
    }
}
