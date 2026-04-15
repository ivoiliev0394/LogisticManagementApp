using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Enums.Assets;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class AirCrewMemberSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.AirCrewMembers.AnyAsync())
                return;

            var companies = await db.Companies.Take(15).ToListAsync();

            var crew = new List<AirCrewMember>();

            var roles = Enum.GetValues<VesselCrewRole>();
            int role = 1;

            for (int i = 0; i < 15 && i < companies.Count; i++)
            {
                crew.Add(new AirCrewMember
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    FullName = $"Air Crew Member {i + 1}",
                    CrewRole = (AirCrewRole)role,
                    LicenseNumber = $"AIR-LIC-{8000 + i}",
                    Phone = $"+35988766{i:000}",
                    IsActive = true,
                    Notes = "Cargo flight crew",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (role < roles.Length)
                    role++;
                else
                    role = 1;
            }

            await db.AirCrewMembers.AddRangeAsync(crew);
            await db.SaveChangesAsync();
        }
    }
}
