using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Clients
{
    public static class ClientProfileSeeder
    {
        public sealed class ClientSeed
        {
            public string UserName { get; init; } = string.Empty;
            public string Email { get; init; } = string.Empty;
            public string PhoneNumber { get; init; } = string.Empty;
            public string FirstName { get; init; } = string.Empty;
            public string LastName { get; init; } = string.Empty;
        }

        public static IReadOnlyList<ClientSeed> Data => new List<ClientSeed>
        {
            new() { UserName = "Ivan_Petrov",      Email = "ivan.petrov@example.com",      PhoneNumber = "+359888111001", FirstName = "Ivan",      LastName = "Petrov" },
            new() { UserName = "Maria_Georgieva",  Email = "maria.georgieva@example.com",  PhoneNumber = "+359888111002", FirstName = "Maria",     LastName = "Georgieva" },
            new() { UserName = "Georgi_Ivanov",    Email = "georgi.ivanov@example.com",    PhoneNumber = "+359888111003", FirstName = "Georgi",    LastName = "Ivanov" },
            new() { UserName = "Petya_Dimitrova",  Email = "petya.dimitrova@example.com",  PhoneNumber = "+359888111004", FirstName = "Petya",     LastName = "Dimitrova" },
            new() { UserName = "Dimitar_Nikolov",  Email = "dimitar.nikolov@example.com",  PhoneNumber = "+359888111005", FirstName = "Dimitar",   LastName = "Nikolov" },
            new() { UserName = "Elena_Todorova",   Email = "elena.todorova@example.com",   PhoneNumber = "+359888111006", FirstName = "Elena",     LastName = "Todorova" },
            new() { UserName = "Nikola_Kolev",     Email = "nikola.kolev@example.com",     PhoneNumber = "+359888111007", FirstName = "Nikola",    LastName = "Kolev" },
            new() { UserName = "Stoyan_Stoyanov",  Email = "stoyan.stoyanov@example.com",  PhoneNumber = "+359888111008", FirstName = "Stoyan",    LastName = "Stoyanov" },
            new() { UserName = "Desislava_Ilieva", Email = "desislava.ilieva@example.com", PhoneNumber = "+359888111009", FirstName = "Desislava", LastName = "Ilieva" },
            new() { UserName = "Rosen_Petkov",     Email = "rosen.petkov@example.com",     PhoneNumber = "+359888111010", FirstName = "Rosen",     LastName = "Petkov" },
            new() { UserName = "Kalina_Hristova",  Email = "kalina.hristova@example.com",  PhoneNumber = "+359888111011", FirstName = "Kalina",    LastName = "Hristova" },
            new() { UserName = "Borislav_Vasilev", Email = "borislav.vasilev@example.com", PhoneNumber = "+359888111012", FirstName = "Borislav",  LastName = "Vasilev" },
            new() { UserName = "Teodora_Angelova", Email = "teodora.angelova@example.com", PhoneNumber = "+359888111013", FirstName = "Teodora",   LastName = "Angelova" },
            new() { UserName = "Hristo_Dimitrov",  Email = "hristo.dimitrov@example.com",  PhoneNumber = "+359888111014", FirstName = "Hristo",    LastName = "Dimitrov" },
            new() { UserName = "Ivaylo_Kostov",    Email = "ivaylo.kostov@example.com",    PhoneNumber = "+359888111015", FirstName = "Ivaylo",    LastName = "Kostov" }
        };

        public static async Task SeedAsync(
            LogisticAppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            var clientsToSeed = Data;

            foreach (var clientSeed in clientsToSeed)
            {
                var user = await userManager.FindByEmailAsync(clientSeed.Email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = clientSeed.UserName,
                        Email = clientSeed.Email,
                        PhoneNumber = clientSeed.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        LockoutEnabled = true
                    };

                    var createResult = await userManager.CreateAsync(user, $"{clientSeed.LastName}123!");

                    if (!createResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create client user '{clientSeed.Email}': " +
                            string.Join("; ", createResult.Errors.Select(e => e.Description)));
                    }
                }

                if (!await userManager.IsInRoleAsync(user, RoleNames.Client))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(user, RoleNames.Client);

                    if (!addToRoleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to assign Client role to '{clientSeed.Email}': " +
                            string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
                    }
                }

                var existingProfile = await db.ClientProfiles
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (existingProfile == null)
                {
                    var profile = new ClientProfile
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        FirstName = clientSeed.FirstName,
                        LastName = clientSeed.LastName,
                        PhoneNumber = clientSeed.PhoneNumber,
                        EmailForContact = clientSeed.Email,
                        CreatedOnUtc = DateTime.UtcNow
                    };

                    await db.ClientProfiles.AddAsync(profile);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}