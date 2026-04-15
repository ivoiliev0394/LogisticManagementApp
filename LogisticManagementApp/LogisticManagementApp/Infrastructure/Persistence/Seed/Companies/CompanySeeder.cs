using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Companies;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Companies
{
    public static class CompanySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Companies.AnyAsync())
                return;

            var companies = new List<Company>
        {
            new() { Id = Guid.NewGuid(), Name = "Balkan Freight Solutions", TaxNumber = "BG204000001", CompanyType = (CompanyType)1, Website = "https://balkanfreight.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Danube River Cargo", TaxNumber = "BG204000002", CompanyType = (CompanyType)2, Website = "https://danubecargo.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Black Sea Logistics", TaxNumber = "BG204000003", CompanyType = (CompanyType)2, Website = "https://blacksea.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Sofia Express Delivery", TaxNumber = "BG204000004", CompanyType = (CompanyType) 3, Website = "https://sofiaexpress.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Plovdiv Warehouse Group", TaxNumber = "BG204000005", CompanyType = (CompanyType) 4, Website = "https://plovdivwarehouse.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Varna Port Services", TaxNumber = "BG204000006", CompanyType = (CompanyType) 5, Website = "https://varnaport.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Euro Rail Cargo", TaxNumber = "RO204000007", CompanyType = (CompanyType) 2, Website = "https://eurorail.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Adriatic Shipping Lines", TaxNumber = "IT204000008", CompanyType = (CompanyType) 2, Website = "https://adriaticshipping.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Mediterranean Air Freight", TaxNumber = "GR204000009", CompanyType = (CompanyType) 2, Website = "https://medairfreight.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Central Europe Trucking", TaxNumber = "HU204000010", CompanyType = (CompanyType) 2, Website = "https://cetrucking.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Burgas Cold Chain", TaxNumber = "BG204000011", CompanyType = (CompanyType) 4, Website = "https://burgascold.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Ruse Intermodal Hub", TaxNumber = "BG204000012", CompanyType = (CompanyType) 5, Website = "https://rusehub.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Hamburg Ocean Connect", TaxNumber = "DE204000013", CompanyType = (CompanyType) 2, Website = "https://hamburgocean.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Rotterdam Container Link", TaxNumber = "NL204000014", CompanyType = (CompanyType) 2, Website = "https://rotterdamlink.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Client Trade Group", TaxNumber = "BG204000015", CompanyType = (CompanyType) 6, Website = "https://clienttrade.example", IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        };

            await db.Companies.AddRangeAsync(companies);
            await db.SaveChangesAsync();
        }
    }
}
