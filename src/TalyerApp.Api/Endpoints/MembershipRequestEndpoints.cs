using TalyerApp.Application.Common.Authorization;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.MembershipRequests;
using TalyerApp.Application.Interfaces;
using TalyerApp.Api.Extensions;
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

        app.MapPost("/membership-requests/confirm-email", async (
            ConfirmMembershipRequestEmailRequest body,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var result = await dispatcher.DispatchAsync<ConfirmMembershipRequestEmailCmd, int>(
                new ConfirmMembershipRequestEmailCmd(body.Token));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(new { id = result.Value });
        })
        .WithName("ConfirmMembershipRequestEmail");

        app.MapGet("/membership-requests", async (
            MembershipRequestStatus? status,
            HttpContext httpContext,
            IPlatformAdminAuthorizer platformAdminAuthorizer,
            IQueryDispatcher queryDispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var auth = await PlatformAdminEndpointAuthorization.EnsurePlatformAdminAsync(
                httpContext,
                platformAdminAuthorizer);
            if (auth is not null)
            {
                return auth;
            }

            var filter = status ?? MembershipRequestStatus.PendingReview;
            var result = await queryDispatcher.DispatchAsync<GetMembershipRequestsQuery, IReadOnlyList<MembershipRequestDto>>(
                new GetMembershipRequestsQuery(filter));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(result.Value);
        })
        .RequireAuthorization()
        .WithName("ListMembershipRequests");

        app.MapPost("/membership-requests/{id:int}/approve", async (
            int id,
            HttpContext httpContext,
            IPlatformAdminAuthorizer platformAdminAuthorizer,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var auth = await PlatformAdminEndpointAuthorization.EnsurePlatformAdminAsync(
                httpContext,
                platformAdminAuthorizer);
            if (auth is not null)
            {
                return auth;
            }

            var result = await dispatcher.DispatchAsync<ApproveMembershipRequestCmd, int>(
                new ApproveMembershipRequestCmd(id));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(new { id = result.Value });
        })
        .RequireAuthorization()
        .WithName("ApproveMembershipRequest");

        app.MapPost("/membership-requests/{id:int}/reject", async (
            int id,
            RejectMembershipRequestRequest? body,
            HttpContext httpContext,
            IPlatformAdminAuthorizer platformAdminAuthorizer,
            ICommandDispatcher dispatcher,
            IErrorMessageResolver messages,
            IErrorHttpStatusMapper statuses) =>
        {
            var auth = await PlatformAdminEndpointAuthorization.EnsurePlatformAdminAsync(
                httpContext,
                platformAdminAuthorizer);
            if (auth is not null)
            {
                return auth;
            }

            var result = await dispatcher.DispatchAsync<RejectMembershipRequestCmd, int>(
                new RejectMembershipRequestCmd(id, body?.Reason));

            return result.IsFailure
                ? result.Error.ToProblemResult(messages, statuses)
                : Results.Ok(new { id = result.Value });
        })
        .RequireAuthorization()
        .WithName("RejectMembershipRequest");

        return app;
    }
}
