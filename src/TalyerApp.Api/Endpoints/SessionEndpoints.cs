using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TalyerApp.Api.Extensions;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.Sessions;
using TalyerApp.Application.Interfaces;
using TalyerApp.Domain.Authorization;
using AuthSchemes = TalyerApp.Domain.Authorization.AuthenticationSchemes;

namespace TalyerApp.Api.Endpoints;

public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/session", async (
            ClaimsPrincipal principal,
            ICommandDispatcher dispatcher,
            ITalyerSessionTokenIssuer tokenIssuer,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses,
            CancellationToken cancellationToken) =>
        {
            var providerUserId = principal.FindFirst("sub")?.Value;
            var email = principal.FindFirst("email")?.Value
                ?? principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(providerUserId) || string.IsNullOrWhiteSpace(email))
            {
                return Results.Unauthorized();
            }

            var result = await dispatcher.DispatchAsync<EstablishSessionCmd, SessionDto>(
                new EstablishSessionCmd(providerUserId, email),
                cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.ToProblemResult(messages, statuses);
            }

            var session = result.Value;
            return Results.Ok(new
            {
                accessToken = tokenIssuer.Issue(session),
                session
            });
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = AuthSchemes.Auth0 })
        .WithName("EstablishSession");

        return app;
    }
}
