using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Models.Auth;

namespace LogisticManagementApp.Applicationn.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string? ErrorMessage, ApplicationUser? User)> LoginAsync(LoginViewModel model);

        Task<(bool Succeeded, string ErrorMessage)> RegisterClientAsync(RegisterViewModel model);

        Task LogoutAsync();

        Task CreateSessionAsync(ApplicationUser user, string? ipAddress, string? userAgent);

        Task CloseActiveSessionsAsync(string userId);

        Task<string> ResolveLandingActionAsync(ApplicationUser user);
    }
}
