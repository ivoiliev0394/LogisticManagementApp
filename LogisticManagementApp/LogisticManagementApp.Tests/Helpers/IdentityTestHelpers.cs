using LogisticManagementApp.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LogisticManagementApp.Tests.Helpers;

internal static class IdentityTestHelpers
{
    public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock(IQueryable<ApplicationUser>? users = null)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(x => x.Value).Returns(new IdentityOptions());

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options.Object,
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        if (users != null)
        {
            userManager.Setup(x => x.Users).Returns(users);
        }

        return userManager;
    }

    public static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<ApplicationUser>>().Object);
    }
}
