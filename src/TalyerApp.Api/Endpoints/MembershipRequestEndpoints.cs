using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.MembershipRequests;
using TalyerApp.Application.Interfaces;
using TalyerApp.Api.Extensions;
using TalyerApp.Domain.Authorization;
using TalyerApp.Domain.Shared.Membership;

namespace TalyerApp.Api.Endpoints;

public static class MembershipRequestEndpoints
{
    public static IEndpointRouteBuilder MapMembershipRequestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/membership-requests", async (
            SubmitMembershipRequestRequest body,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var result = await dispatcher.DispatchAsync<SubmitMembershipRequestCmd, int>(
                new SubmitMembershipRequestCmd(body.CompanyName, body.Email, body.FirstName, body.LastName));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Created($"/membership-requests/{result.Value}", new { id = result.Value });
        })
        .WithName("SubmitMembershipRequest");

        app.MapGet("/membership-requests/confirm-email", async (
            string? token,
            ICommandDispatcher dispatcher,
            IEmailTemplateRenderer templates,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var result = await dispatcher.DispatchAsync<ConfirmMembershipRequestEmailCmd, int>(
                new ConfirmMembershipRequestEmailCmd(token ?? string.Empty));

            return ConfirmEmailHtmlResult.From(result, templates, messages, statuses);
        })
        .WithName("ConfirmMembershipRequestEmail");

        app.MapGet("/membership-requests", async (
            MembershipRequestStatus? status,
            IQueryDispatcher queryDispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var filter = status ?? MembershipRequestStatus.PendingReview;
            var result = await queryDispatcher.DispatchAsync<GetMembershipRequestsQuery, IReadOnlyList<MembershipRequestDto>>(
                new GetMembershipRequestsQuery(filter));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(result.Value);
        })
        .RequireAuthorization(AuthorizationPolicies.PlatformOwner)
        .WithName("ListMembershipRequests");

        app.MapPost("/membership-requests/{id:int}/approve", async (
            int id,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var result = await dispatcher.DispatchAsync<ApproveMembershipRequestCmd, int>(
                new ApproveMembershipRequestCmd(id));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(new { id = result.Value });
        })
        .RequireAuthorization(AuthorizationPolicies.PlatformOwner)
        .WithName("ApproveMembershipRequest");

        app.MapPost("/membership-requests/{id:int}/reject", async (
            int id,
            RejectMembershipRequestRequest? body,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var result = await dispatcher.DispatchAsync<RejectMembershipRequestCmd, int>(
                new RejectMembershipRequestCmd(id, body?.Reason));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(new { id = result.Value });
        })
        .RequireAuthorization(AuthorizationPolicies.PlatformOwner)
        .WithName("RejectMembershipRequest");

        return app;
    }
}
