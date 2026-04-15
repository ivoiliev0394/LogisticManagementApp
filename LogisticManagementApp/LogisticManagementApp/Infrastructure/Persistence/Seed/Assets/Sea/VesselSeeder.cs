using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class VesselSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Vessels.AnyAsync())
                return;

            var companyId = await db.Companies
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (companyId == Guid.Empty)
                return; // или throw new Exception("No companies found for vessel seeding.");

            var vessels = new List<Vessel>();

            var types = Enum.GetValues<VesselType>();
            int type = 1;
            var statuses = Enum.GetValues<AssetStatus>();
            int status = 1;

            for (int i = 0; i < 15; i++)
            {
                vessels.Add(new Vessel
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    Name = $"MV Horizon {i + 1}",
                    ImoNumber = $"IMO{9000000 + i}",
                    MmsiNumber = $"{200000000 + i}",
                    VesselType = (VesselType)type, // ако съществува в enum-а
                    CapacityTeu = 3000 + (i * 150),
                    DeadweightTons = 50000 + (i * 2000),
                    Status = (AssetStatus)status,
                    IsActive = true,
                    Notes = $"Seeded vessel #{i + 1}"
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
                if (status < statuses.Length)
                    status++;
                else
                    status = 1;
            }

            await db.Vessels.AddRangeAsync(vessels);
            await db.SaveChangesAsync();
        }
    }
}
