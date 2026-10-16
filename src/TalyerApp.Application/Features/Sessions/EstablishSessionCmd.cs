using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Dto;

namespace TalyerApp.Application.Features.Sessions;

public sealed record EstablishSessionCmd(string ProviderUserId, string Email) : ICommand<SessionDto>;
