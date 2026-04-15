using LogisticManagementApp.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Clients
{
    public static class ClientAddressSeeder
    {
        public sealed class ClientAddressSeed
        {
            public string Email { get; init; } = string.Empty;
            public string Country { get; init; } = string.Empty;
            public string City { get; init; } = string.Empty;
            public string Street { get; init; } = string.Empty;
            public string PostalCode { get; init; } = string.Empty;
            public bool IsDefault { get; init; }
        }

        public static IReadOnlyList<ClientAddressSeed> Data => new List<ClientAddressSeed>
        {
            new() { Email = "ivan.petrov@example.com",      Country = "Bulgaria", City = "Sofia",          Street = "bul. Vitosha 1",            PostalCode = "1000", IsDefault = true },
            new() { Email = "maria.georgieva@example.com",  Country = "Bulgaria", City = "Plovdiv",        Street = "ul. Gladston 15",           PostalCode = "4000", IsDefault = true },
            new() { Email = "georgi.ivanov@example.com",    Country = "Bulgaria", City = "Varna",          Street = "bul. Slivnitsa 45",         PostalCode = "9000", IsDefault = true },
            new() { Email = "petya.dimitrova@example.com",  Country = "Bulgaria", City = "Burgas",         Street = "ul. Aleksandrovska 55",     PostalCode = "8000", IsDefault = true },
            new() { Email = "dimitar.nikolov@example.com",  Country = "Bulgaria", City = "Ruse",           Street = "bul. Tsar Osvoboditel 23",  PostalCode = "7000", IsDefault = true },
            new() { Email = "elena.todorova@example.com",   Country = "Bulgaria", City = "Stara Zagora",   Street = "ul. General Stoletov 10",   PostalCode = "6000", IsDefault = true },
            new() { Email = "nikola.kolev@example.com",     Country = "Bulgaria", City = "Pleven",         Street = "ul. Vasil Levski 78",       PostalCode = "5800", IsDefault = true },
            new() { Email = "stoyan.stoyanov@example.com",  Country = "Bulgaria", City = "Veliko Tarnovo", Street = "ul. Gurko 12",              PostalCode = "5000", IsDefault = true },
            new() { Email = "desislava.ilieva@example.com", Country = "Bulgaria", City = "Blagoevgrad",    Street = "ul. Todor Alexandrov 5",    PostalCode = "2700", IsDefault = true },
            new() { Email = "rosen.petkov@example.com",     Country = "Bulgaria", City = "Shumen",         Street = "ul. Tsar Osvoboditel 90",   PostalCode = "9700", IsDefault = true },
            new() { Email = "kalina.hristova@example.com",  Country = "Bulgaria", City = "Dobrich",        Street = "ul. Bulgaria 20",           PostalCode = "9300", IsDefault = true },
            new() { Email = "borislav.vasilev@example.com", Country = "Bulgaria", City = "Haskovo",        Street = "ul. San Stefano 33",        PostalCode = "6300", IsDefault = true },
            new() { Email = "teodora.angelova@example.com", Country = "Bulgaria", City = "Yambol",         Street = "ul. Rakovski 11",           PostalCode = "8600", IsDefault = true },
            new() { Email = "hristo.dimitrov@example.com",  Country = "Bulgaria", City = "Pernik",         Street = "ul. Krakra 8",              PostalCode = "2300", IsDefault = true },
            new() { Email = "ivaylo.kostov@example.com",    Country = "Bulgaria", City = "Sliven",         Street = "ul. Hadzhi Dimitar 14",     PostalCode = "8800", IsDefault = true }
        };

        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ClientAddresses.AnyAsync())
                return;

            var profiles = await db.ClientProfiles
                .ToDictionaryAsync(x => x.EmailForContact!, x => x);

            var addresses = new List<ClientAddress>();

            foreach (var seed in Data)
            {
                if (!profiles.TryGetValue(seed.Email, out var profile))
                    continue;

                addresses.Add(new ClientAddress
                {
                    Id = Guid.NewGuid(),
                    ClientProfileId = profile.Id,
                    Country = seed.Country,
                    City = seed.City,
                    Street = seed.Street,
                    PostalCode = seed.PostalCode,
                    IsDefault = seed.IsDefault
                });
            }

            if (addresses.Count == 0)
                return;

            await db.ClientAddresses.AddRangeAsync(addresses);
            await db.SaveChangesAsync();
        }
    }
}