using LogisticManagementApp.Applicationn.Interfaces.Auth;
using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Domain.Security;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly LogisticAppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            LogisticAppDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Succeeded, string? ErrorMessage, ApplicationUser? User)> LoginAsync(LoginViewModel model)
        {
            var normalizedInput = model.UsernameOrEmail.Trim().ToUpperInvariant();

            var user = await _userManager.Users
                .Include(x => x.ClientProfile)
                .FirstOrDefaultAsync(x =>
                    x.NormalizedUserName == normalizedInput ||
                    x.NormalizedEmail == normalizedInput);

            if (user == null)
            {
                return (false, "Невалидно потребителско име/имейл или парола.", null);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return (false, "Акаунтът е временно заключен. Опитай по-късно.", null);
            }

            if (!result.Succeeded)
            {
                return (false, "Невалидно потребителско име/имейл или парола.", null);
            }

            return (true, null, user);
        }

        public async Task<(bool Succeeded, string ErrorMessage)> RegisterClientAsync(RegisterViewModel model)
        {
            var username = model.Username.Trim();
            var email = model.Email.Trim();
            var phoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber)
                ? null
                : model.PhoneNumber.Trim();

            var existingByUsername = await _userManager.FindByNameAsync(username);
            if (existingByUsername != null)
            {
                return (false, "Потребителското име вече съществува.");
            }

            var existingByEmail = await _userManager.FindByEmailAsync(email);
            if (existingByEmail != null)
            {
                return (false, "Имейл адресът вече съществува.");
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(phoneNumber),
                    LockoutEnabled = true
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);

                if (!createResult.Succeeded)
                {
                    var error = createResult.Errors.FirstOrDefault()?.Description
                        ?? "Възникна проблем при регистрацията.";
                    return (false, error);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Client);
                if (!roleResult.Succeeded)
                {
                    var error = roleResult.Errors.FirstOrDefault()?.Description
                        ?? "Неуспешно задаване на роля.";
                    return (false, error);
                }

                var profile = new ClientProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    PhoneNumber = phoneNumber,
                    EmailForContact = email,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _db.ClientProfiles.AddAsync(profile);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, string.Empty);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Възникна проблем при регистрацията. Опитай отново.");
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task CreateSessionAsync(ApplicationUser user, string? ipAddress, string? userAgent)
        {
            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SessionToken = Guid.NewGuid().ToString("N"),
                StartedAtUtc = DateTime.UtcNow,
                LastSeenAtUtc = DateTime.UtcNow,
                Status = Domain.Enums.Operations.UserSessionStatus.Active,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _db.UserSessions.AddAsync(session);
            await _db.SaveChangesAsync();
        }

        public async Task CloseActiveSessionsAsync(string userId)
        {
            var sessions = await _db.UserSessions
                .Where(x => x.UserId == userId &&
                            x.Status == Domain.Enums.Operations.UserSessionStatus.Active)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.Status = Domain.Enums.Operations.UserSessionStatus.Expired;
                session.ExpiresAtUtc = DateTime.UtcNow;
                session.LastSeenAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<string> ResolveLandingActionAsync(ApplicationUser user)
        {
            if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
            {
                return "/Home/AdminDashboard";
            }

            if (await _userManager.IsInRoleAsync(user, RoleNames.Company))
            {
                return "/Home/CompanyDashboard";
            }

            if (await _userManager.IsInRoleAsync(user, RoleNames.Client))
            {
                return "/ClientPortal/Dashboard";
            }

            return "/";
        }
    }
}