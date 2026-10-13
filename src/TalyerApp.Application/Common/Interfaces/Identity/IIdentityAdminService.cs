using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Identity;

public interface IIdentityAdminService
{
    Task<Result<string>> CreateUserAsync(
        string email,
        string username,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task DeleteUserAsync(string providerUserId, CancellationToken cancellationToken = default);

    Task<Result> SendExecuteActionsEmailAsync(
        string providerUserId,
        IReadOnlyList<string> actions,
        CancellationToken cancellationToken = default);
}
