using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class VesselCrewMemberSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.VesselCrewMembers.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var crew = new List<VesselCrewMember>();

            var roles = Enum.GetValues<VesselCrewRole>();
            int role = 1;


            for (int i = 0; i < 15; i++)
            {
                var company = companies[i % companies.Count];

                crew.Add(new VesselCrewMember
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    FullName = $"John{i} Doe{i}",
                    CrewRole = (VesselCrewRole)role,
                    SeamanBookNumber = $"SB-{1000 + i}",
                    CertificateNumber = $"CERT-{2000 + i}",
                    Phone = $"+359888000{i:D3}",
                    Status = AssetStatus.Available,
                    IsActive = true,
                    Notes = "Seeded crew member",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (role < roles.Length)
                    role++;
                else
                    role = 1;
            }

            await db.VesselCrewMembers.AddRangeAsync(crew);
            await db.SaveChangesAsync();
        }
    }
}
