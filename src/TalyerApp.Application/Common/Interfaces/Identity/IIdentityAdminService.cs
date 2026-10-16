using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Identity;

public interface IIdentityAdminService
{
    string Provider { get; }

    Task<Result<CreatedIdentityUser>> CreateUserAsync(
        CreateIdentityUserRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(string providerUserId, CancellationToken cancellationToken = default);
}
