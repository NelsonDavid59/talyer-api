using TalyerApp.Application.Common.Interfaces.CQRS;

namespace TalyerApp.Application.Features.ExternalIdentities;

public sealed record RegisterExternalIdentityCmd(
    string Provider,
    string ProviderUserId,
    string Username,
    string FirstName,
    string LastName,
    string Email
) : ICommand<Guid>;