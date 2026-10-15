using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Common.Interfaces.Notifications;

public interface IMembershipEmailService
{
    Task<Result> SendConfirmRequestAsync(
        MembershipRequest request,
        string plainToken,
        CancellationToken cancellationToken = default);

    Task<Result> SendEmailConfirmedAsync(
        MembershipRequest request,
        CancellationToken cancellationToken = default);
}
