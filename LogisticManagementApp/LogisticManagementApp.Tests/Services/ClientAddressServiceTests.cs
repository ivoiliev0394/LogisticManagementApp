using LogisticManagementApp.Applicationn.Services.ClientPortal;
using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Models.ClientPortal;
using LogisticManagementApp.Tests.Helpers;

namespace LogisticManagementApp.Tests.Services;

public class ClientAddressServiceTests
{
    [Fact]
    public async Task CreateAddressAsync_MakesFirstAddressDefault_EvenWhenModelSaysFalse()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");
        var service = new ClientAddressService(db);

        var model = new ClientAddressFormViewModel
        {
            Country = " Bulgaria ",
            City = " Sofia ",
            Street = " Blvd 1 ",
            PostalCode = " 1000 ",
            IsDefault = false
        };

        await service.CreateAddressAsync("user-1", model);

        var saved = Assert.Single(db.ClientAddresses);
        Assert.True(saved.IsDefault);
        Assert.Equal("Bulgaria", saved.Country);
        Assert.Equal("Sofia", saved.City);
        Assert.Equal("Blvd 1", saved.Street);
        Assert.Equal("1000", saved.PostalCode);
        Assert.Equal(profile.Id, saved.ClientProfileId);
    }

    [Fact]
    public async Task CreateAddressAsync_WhenNewAddressIsDefault_UnsetsPreviousDefault()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");
        db.ClientAddresses.Add(new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Plovdiv",
            Street = "Old",
            IsDefault = true
        });
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        await service.CreateAddressAsync("user-1", new ClientAddressFormViewModel
        {
            Country = "Germany",
            City = "Berlin",
            Street = "New",
            IsDefault = true
        });

        Assert.Equal(2, db.ClientAddresses.Count());
        Assert.Single(db.ClientAddresses.Where(x => x.IsDefault && x.City == "Berlin"));
        Assert.Single(db.ClientAddresses.Where(x => !x.IsDefault && x.City == "Plovdiv"));
    }

    [Fact]
    public async Task UpdateAddressAsync_KeepsOnlyAddressAsDefault_WhenUserAttemptsToUnsetIt()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");
        var address = new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Sofia",
            Street = "Street 1",
            IsDefault = true
        };
        db.ClientAddresses.Add(address);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);
        var model = new ClientAddressFormViewModel
        {
            Id = address.Id,
            Country = "Bulgaria",
            City = "Sofia",
            Street = "Street 2",
            PostalCode = null,
            IsDefault = false
        };

        var updated = await service.UpdateAddressAsync("user-1", model);

        Assert.True(updated);
        var saved = await db.ClientAddresses.FindAsync(address.Id);
        Assert.NotNull(saved);
        Assert.True(saved!.IsDefault);
        Assert.Equal("Street 2", saved.Street);
    }

    [Fact]
    public async Task DeleteAddressAsync_WhenDeletingDefault_PromotesNextAddressAlphabetically()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");

        var defaultAddress = new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Varna",
            Street = "Z street",
            IsDefault = true
        };
        var promotedAddress = new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Burgas",
            Street = "A street",
            IsDefault = false
        };

        db.ClientAddresses.AddRange(defaultAddress, promotedAddress);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        var deleted = await service.DeleteAddressAsync("user-1", defaultAddress.Id);

        Assert.True(deleted);
        var remaining = Assert.Single(db.ClientAddresses);
        Assert.Equal(promotedAddress.Id, remaining.Id);
        Assert.True(remaining.IsDefault);
    }

    [Fact]
    public async Task GetAddressesAsync_ReturnsOnlyCurrentUserAddresses_OrderedWithDefaultFirst()
    {
        await using var db = TestDbContextFactory.Create();
        var firstProfile = await SeedClientProfileAsync(db, "user-1");
        var secondProfile = await SeedClientProfileAsync(db, "user-2");

        db.ClientAddresses.AddRange(
            new ClientAddress
            {
                ClientProfileId = firstProfile.Id,
                Country = "Bulgaria",
                City = "Varna",
                Street = "B street",
                IsDefault = false
            },
            new ClientAddress
            {
                ClientProfileId = firstProfile.Id,
                Country = "Bulgaria",
                City = "Burgas",
                Street = "A street",
                IsDefault = true
            },
            new ClientAddress
            {
                ClientProfileId = secondProfile.Id,
                Country = "Romania",
                City = "Constanta",
                Street = "Other",
                IsDefault = true
            });

        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        var result = await service.GetAddressesAsync("user-1");

        Assert.Equal(2, result.Addresses.Count);
        Assert.True(result.Addresses[0].IsDefault);
        Assert.Equal("Burgas", result.Addresses[0].City);
        Assert.Equal("Varna", result.Addresses[1].City);
    }

    [Fact]
    public async Task SetDefaultAddressAsync_SetsTargetAndUnsetsExistingDefault()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");
        var first = new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Sofia",
            Street = "First",
            IsDefault = true
        };
        var second = new ClientAddress
        {
            ClientProfileId = profile.Id,
            Country = "Bulgaria",
            City = "Plovdiv",
            Street = "Second",
            IsDefault = false
        };
        db.ClientAddresses.AddRange(first, second);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        var result = await service.SetDefaultAddressAsync("user-1", second.Id);

        Assert.True(result);
        Assert.False((await db.ClientAddresses.FindAsync(first.Id))!.IsDefault);
        Assert.True((await db.ClientAddresses.FindAsync(second.Id))!.IsDefault);
    }


    [Fact]
    public async Task GetAddressForEditAsync_ReturnsOnlyOwnedAddress()
    {
        await using var db = TestDbContextFactory.Create();
        var firstProfile = await SeedClientProfileAsync(db, "user-1");
        var secondProfile = await SeedClientProfileAsync(db, "user-2");
        var owned = new ClientAddress { ClientProfileId = firstProfile.Id, Country = "BG", City = "Sofia", Street = "Owned", IsDefault = true };
        var foreign = new ClientAddress { ClientProfileId = secondProfile.Id, Country = "BG", City = "Plovdiv", Street = "Foreign", IsDefault = true };
        db.ClientAddresses.AddRange(owned, foreign);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        var result = await service.GetAddressForEditAsync("user-1", owned.Id);
        var denied = await service.GetAddressForEditAsync("user-1", foreign.Id);

        Assert.NotNull(result);
        Assert.Equal("Owned", result!.Street);
        Assert.Null(denied);
    }

    [Fact]
    public async Task UpdateAddressAsync_WhenSettingAnotherAddressDefault_UnsetsPreviousDefault()
    {
        await using var db = TestDbContextFactory.Create();
        var profile = await SeedClientProfileAsync(db, "user-1");
        var first = new ClientAddress { ClientProfileId = profile.Id, Country = "BG", City = "Sofia", Street = "First", IsDefault = true };
        var second = new ClientAddress { ClientProfileId = profile.Id, Country = "BG", City = "Varna", Street = "Second", IsDefault = false };
        db.ClientAddresses.AddRange(first, second);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);
        var updated = await service.UpdateAddressAsync("user-1", new ClientAddressFormViewModel
        {
            Id = second.Id,
            Country = "BG",
            City = "Varna",
            Street = "Second Updated",
            IsDefault = true
        });

        Assert.True(updated);
        Assert.False((await db.ClientAddresses.FindAsync(first.Id))!.IsDefault);
        Assert.True((await db.ClientAddresses.FindAsync(second.Id))!.IsDefault);
    }

    [Fact]
    public async Task DeleteAddressAsync_ReturnsFalse_WhenAddressIsMissingOrForeign()
    {
        await using var db = TestDbContextFactory.Create();
        var firstProfile = await SeedClientProfileAsync(db, "user-1");
        var secondProfile = await SeedClientProfileAsync(db, "user-2");
        var foreign = new ClientAddress { ClientProfileId = secondProfile.Id, Country = "BG", City = "Plovdiv", Street = "Foreign", IsDefault = true };
        db.ClientAddresses.Add(foreign);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        Assert.False(await service.DeleteAddressAsync("user-1", Guid.NewGuid()));
        Assert.False(await service.DeleteAddressAsync("user-1", foreign.Id));
        Assert.Single(db.ClientAddresses);
    }

    [Fact]
    public async Task SetDefaultAddressAsync_ReturnsFalse_WhenTargetDoesNotBelongToUser()
    {
        await using var db = TestDbContextFactory.Create();
        var firstProfile = await SeedClientProfileAsync(db, "user-1");
        var secondProfile = await SeedClientProfileAsync(db, "user-2");
        var foreign = new ClientAddress { ClientProfileId = secondProfile.Id, Country = "BG", City = "Plovdiv", Street = "Foreign", IsDefault = true };
        db.ClientAddresses.Add(foreign);
        await db.SaveChangesAsync();

        var service = new ClientAddressService(db);

        var result = await service.SetDefaultAddressAsync("user-1", foreign.Id);

        Assert.False(result);
    }

    private static async Task<ClientProfile> SeedClientProfileAsync(LogisticManagementApp.Infrastructure.Persistence.LogisticAppDbContext db, string userId)
    {
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userId,
            NormalizedUserName = userId.ToUpperInvariant(),
            Email = $"{userId}@example.com",
            NormalizedEmail = $"{userId}@example.com".ToUpperInvariant()
        };

        var profile = new ClientProfile
        {
            UserId = userId,
            User = user,
            FirstName = "Ivan",
            LastName = "Ivanov",
            EmailForContact = "ivan@example.com"
        };

        db.Users.Add(user);
        db.ClientProfiles.Add(profile);
        await db.SaveChangesAsync();
        return profile;
    }
}
