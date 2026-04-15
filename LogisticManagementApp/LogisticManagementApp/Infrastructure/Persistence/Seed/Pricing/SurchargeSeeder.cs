using LogisticManagementApp.Domain.Enums.Pricing;
using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class SurchargeSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Surcharges.AnyAsync())
                return;

            var surcharges = new List<Surcharge>
        {
            new() { Id = Guid.NewGuid(), Code = "FUEL", Name = "Fuel Surcharge", SurchargeType = SurchargeType.Percent, Value = 12.5m },
            new() { Id = Guid.NewGuid(), Code = "OVERSIZE", Name = "Oversize Cargo", SurchargeType = SurchargeType.Fixed, Value = 150m },
            new() { Id = Guid.NewGuid(), Code = "FRAGILE", Name = "Fragile Handling", SurchargeType = SurchargeType.Fixed, Value = 75m },
            new() { Id = Guid.NewGuid(), Code = "HAZMAT", Name = "Hazardous Goods", SurchargeType = SurchargeType.Fixed, Value = 300m },
            new() { Id = Guid.NewGuid(), Code = "REEFER", Name = "Refrigerated Cargo", SurchargeType = SurchargeType.Fixed, Value = 200m },
            new() { Id = Guid.NewGuid(), Code = "WEEKEND", Name = "Weekend Delivery", SurchargeType = SurchargeType.Fixed, Value = 50m },
            new() { Id = Guid.NewGuid(), Code = "EXPRESS", Name = "Express Priority", SurchargeType = SurchargeType.Percent, Value = 25m },
            new() { Id = Guid.NewGuid(), Code = "SECURITY", Name = "Security Fee", SurchargeType = SurchargeType.Fixed, Value = 20m },
            new() { Id = Guid.NewGuid(), Code = "PORT", Name = "Port Handling", SurchargeType = SurchargeType.Fixed, Value = 100m },
            new() { Id = Guid.NewGuid(), Code = "AIRPORT", Name = "Airport Handling", SurchargeType = SurchargeType.Fixed, Value = 120m },
            new() { Id = Guid.NewGuid(), Code = "CUSTOMS", Name = "Customs Processing", SurchargeType = SurchargeType.Fixed, Value = 80m },
            new() { Id = Guid.NewGuid(), Code = "INSURANCE", Name = "Insurance Fee", SurchargeType = SurchargeType.Percent, Value = 3m },
            new() { Id = Guid.NewGuid(), Code = "PEAK", Name = "Peak Season", SurchargeType = SurchargeType.Percent, Value = 18m },
            new() { Id = Guid.NewGuid(), Code = "REMOTE", Name = "Remote Area", SurchargeType = SurchargeType.Fixed, Value = 90m },
            new() { Id = Guid.NewGuid(), Code = "LIFTGATE", Name = "Liftgate Service", SurchargeType = SurchargeType.Fixed, Value = 60m }
        };

            await db.Surcharges.AddRangeAsync(surcharges);
            await db.SaveChangesAsync();
        }
    }
}
