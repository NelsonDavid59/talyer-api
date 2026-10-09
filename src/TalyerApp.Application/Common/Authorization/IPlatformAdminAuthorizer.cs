using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Authorization;

public interface IPlatformAdminAuthorizer
{
    Task<Result> EnsureIsPlatformAdminAsync(Guid userId, CancellationToken cancellationToken = default);
}
