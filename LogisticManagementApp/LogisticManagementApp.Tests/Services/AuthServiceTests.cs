using LogisticManagementApp.Application.Services;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Domain.Security;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Models.Auth;
using LogisticManagementApp.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ReturnsError_WhenUserDoesNotExist()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock(db.Users);
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.LoginAsync(new LoginViewModel
        {
            UsernameOrEmail = "missing@example.com",
            Password = "Secret123!"
        });

        Assert.False(result.Succeeded);
        Assert.Null(result.User);
        Assert.Contains("Невалидно", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_ReturnsLockedOutMessage_WhenSignInManagerReportsLockout()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new ApplicationUser { Id = "u1", UserName = "demo", NormalizedUserName = "DEMO", Email = "demo@example.com", NormalizedEmail = "DEMO@EXAMPLE.COM" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var userManager = IdentityTestHelpers.CreateUserManagerMock(db.Users);
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        signInManager
            .Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), "Secret123!", true, true))
            .ReturnsAsync(SignInResult.LockedOut);

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.LoginAsync(new LoginViewModel
        {
            UsernameOrEmail = " demo@example.com ",
            Password = "Secret123!",
            RememberMe = true
        });

        Assert.False(result.Succeeded);
        Assert.Equal("Акаунтът е временно заключен. Опитай по-късно.", result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_ReturnsUser_WhenCredentialsAreValid()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new ApplicationUser { Id = "u1", UserName = "demo", NormalizedUserName = "DEMO", Email = "demo@example.com", NormalizedEmail = "DEMO@EXAMPLE.COM" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var userManager = IdentityTestHelpers.CreateUserManagerMock(db.Users);
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        signInManager
            .Setup(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), "Secret123!", false, true))
            .ReturnsAsync(SignInResult.Success);

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.LoginAsync(new LoginViewModel
        {
            UsernameOrEmail = "demo",
            Password = "Secret123!"
        });

        Assert.True(result.Succeeded);
        Assert.Null(result.ErrorMessage);
        Assert.Same(user, result.User);
    }

    [Fact]
    public async Task RegisterClientAsync_ReturnsError_WhenUsernameAlreadyExists()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        userManager.Setup(x => x.FindByNameAsync("demo")).ReturnsAsync(new ApplicationUser());

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.RegisterClientAsync(CreateRegisterModel());

        Assert.False(result.Succeeded);
        Assert.Equal("Потребителското име вече съществува.", result.ErrorMessage);
    }

    [Fact]
    public async Task RegisterClientAsync_ReturnsError_WhenEmailAlreadyExists()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        userManager.Setup(x => x.FindByNameAsync("demo")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.FindByEmailAsync("demo@example.com")).ReturnsAsync(new ApplicationUser());

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.RegisterClientAsync(CreateRegisterModel());

        Assert.False(result.Succeeded);
        Assert.Equal("Имейл адресът вече съществува.", result.ErrorMessage);
    }

    [Fact]
    public async Task RegisterClientAsync_CreatesProfile_WhenUserAndRoleCreationSucceed()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);

        userManager.Setup(x => x.FindByNameAsync("demo")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.FindByEmailAsync("demo@example.com")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Secret123!"))
            .Callback<ApplicationUser, string>((user, _) => { user.Id = "created-user"; db.Users.Add(user); })
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.Client))
            .ReturnsAsync(IdentityResult.Success);

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.RegisterClientAsync(CreateRegisterModel());

        Assert.True(result.Succeeded);
        var profile = await db.ClientProfiles.SingleAsync();
        Assert.Equal("Demo", profile.FirstName);
        Assert.Equal("User", profile.LastName);
        Assert.Equal("demo@example.com", profile.EmailForContact);
        Assert.Equal("0888123456", profile.PhoneNumber);
    }

    [Fact]
    public async Task CreateSessionAsync_AndCloseActiveSessionsAsync_PersistAndExpireOnlyActiveSessions()
    {
        await using var db = TestDbContextFactory.Create();
        db.Users.Add(new ApplicationUser { Id = "u1", UserName = "demo", Email = "demo@example.com" });
        db.UserSessions.Add(new UserSession
        {
            UserId = "u1",
            SessionToken = "old",
            Status = UserSessionStatus.Expired,
            StartedAtUtc = DateTime.UtcNow.AddHours(-2),
            LastSeenAtUtc = DateTime.UtcNow.AddHours(-1),
            ExpiresAtUtc = DateTime.UtcNow.AddHours(-1)
        });
        await db.SaveChangesAsync();

        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        var service = new AuthService(db, userManager.Object, signInManager.Object);

        await service.CreateSessionAsync(new ApplicationUser { Id = "u1" }, "127.0.0.1", "Tests");
        await service.CloseActiveSessionsAsync("u1");

        var sessions = await db.UserSessions.Where(x => x.UserId == "u1").ToListAsync();
        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, x => Assert.Equal(UserSessionStatus.Expired, x.Status));
        Assert.Contains(sessions, x => x.IpAddress == "127.0.0.1" && x.UserAgent == "Tests" && x.ExpiresAtUtc != null);
    }

    [Theory]
    [InlineData(RoleNames.Admin, "/Home/AdminDashboard")]
    [InlineData(RoleNames.Company, "/Home/CompanyDashboard")]
    [InlineData(RoleNames.Client, "/ClientPortal/Dashboard")]
    public async Task ResolveLandingActionAsync_ReturnsExpectedPath_ForKnownRole(string role, string expectedPath)
    {
        await using var db = TestDbContextFactory.Create();
        var user = new ApplicationUser { Id = "u1" };
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);

        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Admin)).ReturnsAsync(role == RoleNames.Admin);
        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Company)).ReturnsAsync(role == RoleNames.Company);
        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Client)).ReturnsAsync(role == RoleNames.Client);

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.ResolveLandingActionAsync(user);

        Assert.Equal(expectedPath, result);
    }


    [Fact]
    public async Task LogoutAsync_CallsSignOutAsync()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);
        var service = new AuthService(db, userManager.Object, signInManager.Object);

        await service.LogoutAsync();

        signInManager.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task ResolveLandingActionAsync_ReturnsRoot_WhenUserHasNoKnownRole()
    {
        await using var db = TestDbContextFactory.Create();
        var user = new ApplicationUser { Id = "u1" };
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);

        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Admin)).ReturnsAsync(false);
        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Company)).ReturnsAsync(false);
        userManager.Setup(x => x.IsInRoleAsync(user, RoleNames.Client)).ReturnsAsync(false);

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.ResolveLandingActionAsync(user);

        Assert.Equal("/", result);
    }


    [Fact]
    public async Task RegisterClientAsync_ReturnsCreateError_WhenCreateAsyncFails()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);

        userManager.Setup(x => x.FindByNameAsync("demo")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.FindByEmailAsync("demo@example.com")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Secret123!"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "bad password" }));

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.RegisterClientAsync(CreateRegisterModel());

        Assert.False(result.Succeeded);
        Assert.Equal("bad password", result.ErrorMessage);
        Assert.Empty(await db.ClientProfiles.ToListAsync());
    }

    [Fact]
    public async Task RegisterClientAsync_ReturnsRoleError_WhenRoleAssignmentFails()
    {
        await using var db = TestDbContextFactory.Create();
        var userManager = IdentityTestHelpers.CreateUserManagerMock();
        var signInManager = IdentityTestHelpers.CreateSignInManagerMock(userManager.Object);

        userManager.Setup(x => x.FindByNameAsync("demo")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.FindByEmailAsync("demo@example.com")).ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Secret123!"))
            .Callback<ApplicationUser, string>((user, _) => { user.Id = "created-user"; db.Users.Add(user); })
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.Client))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "role failed" }));

        var service = new AuthService(db, userManager.Object, signInManager.Object);

        var result = await service.RegisterClientAsync(CreateRegisterModel());

        Assert.False(result.Succeeded);
        Assert.Equal("role failed", result.ErrorMessage);
    }

    private static RegisterViewModel CreateRegisterModel() => new()
    {
        FirstName = " Demo ",
        LastName = " User ",
        Username = "demo",
        Email = "demo@example.com",
        PhoneNumber = " 0888123456 ",
        Password = "Secret123!",
        ConfirmPassword = "Secret123!"
    };
}
