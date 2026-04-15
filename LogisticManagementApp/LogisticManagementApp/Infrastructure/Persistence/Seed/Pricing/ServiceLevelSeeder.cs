using LogisticManagementApp.Domain.Enums.Pricing;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing
{
    public static class ServiceLevelSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ServiceLevels.AnyAsync())
                return;

            var levels = new List<ServiceLevel>
        {
            new() { Id = Guid.NewGuid(), Code = "ROAD-STD", Name = "Road Standard", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Road, MaxWeightKg = 24000m, EstimatedTransitDays = 3, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "ROAD-EXP", Name = "Road Express", ServiceLevelType = ServiceLevelType.Express, TransportMode = TransportMode.Road, MaxWeightKg = 12000m, EstimatedTransitDays = 1, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "ROAD-SDD", Name = "Road Same Day", ServiceLevelType = ServiceLevelType.SameDay, TransportMode = TransportMode.Road, MaxWeightKg = 5000m, EstimatedTransitDays = 0, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "SEA-FCL", Name = "Sea FCL", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Sea, MaxWeightKg = 26000m, EstimatedTransitDays = 18, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "SEA-LCL", Name = "Sea LCL", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Sea, MaxWeightKg = 12000m, EstimatedTransitDays = 22, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "SEA-EXP", Name = "Sea Priority", ServiceLevelType = ServiceLevelType.Express, TransportMode = TransportMode.Sea, MaxWeightKg = 15000m, EstimatedTransitDays = 14, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "AIR-STD", Name = "Air Standard", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Air, MaxWeightKg = 3000m, EstimatedTransitDays = 2, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "AIR-EXP", Name = "Air Express", ServiceLevelType = ServiceLevelType.Express, TransportMode = TransportMode.Air, MaxWeightKg = 1500m, EstimatedTransitDays = 1, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "AIR-PRM", Name = "Air Premium", ServiceLevelType = ServiceLevelType.Premium, TransportMode = TransportMode.Air, MaxWeightKg = 1000m, EstimatedTransitDays = 1, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "RAIL-STD", Name = "Rail Standard", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Rail, MaxWeightKg = 30000m, EstimatedTransitDays = 5, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "RAIL-EXP", Name = "Rail Express", ServiceLevelType = ServiceLevelType.Express, TransportMode = TransportMode.Rail, MaxWeightKg = 22000m, EstimatedTransitDays = 3, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "COURIER-STD", Name = "Courier Standard", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.InlandWaterway, MaxWeightKg = 30m, EstimatedTransitDays = 2, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "COURIER-EXP", Name = "Courier Express", ServiceLevelType = ServiceLevelType.Express, TransportMode = TransportMode.InlandWaterway, MaxWeightKg = 20m, EstimatedTransitDays = 1, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "INT-MULTI", Name = "Intermodal Standard", ServiceLevelType = ServiceLevelType.Standard, TransportMode = TransportMode.Multimodal, MaxWeightKg = 28000m, EstimatedTransitDays = 8, IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "INT-PRM", Name = "Intermodal Premium", ServiceLevelType = ServiceLevelType.Premium, TransportMode = TransportMode.Multimodal, MaxWeightKg = 18000m, EstimatedTransitDays = 5, IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        };

            await db.ServiceLevels.AddRangeAsync(levels);
            await db.SaveChangesAsync();
        }
    }
}
