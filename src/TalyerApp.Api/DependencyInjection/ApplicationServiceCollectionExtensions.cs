using TalyerApp.Application.Common.Authorization;
using TalyerApp.Application.Common.Dispatching;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Features.MembershipRequests;
using TalyerApp.Application.Features.UserRoleAssignments;
using TalyerApp.Application.Interfaces;

namespace TalyerApp.Api.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MembershipOptions>(
            configuration.GetSection(MembershipOptions.SectionName));

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<ICommandHandler<RegisterExternalIdentityCmd, Guid>, RegisterExternalIdentityCmdHdlr>();
        services.AddScoped<ICommandHandler<AssignUserRoleCmd, int>, AssignUserRoleCmdHdlr>();
        services.AddScoped<ICommandHandler<SubmitMembershipRequestCmd, int>, SubmitMembershipRequestCmdHdlr>();
        services.AddScoped<ICommandHandler<ConfirmMembershipRequestEmailCmd, int>, ConfirmMembershipRequestEmailCmdHdlr>();
        services.AddScoped<ICommandHandler<ApproveMembershipRequestCmd, int>, ApproveMembershipRequestCmdHdlr>();
        services.AddScoped<ICommandHandler<RejectMembershipRequestCmd, int>, RejectMembershipRequestCmdHdlr>();
        services.AddScoped<IQueryHandler<GetMembershipRequestsQuery, IReadOnlyList<MembershipRequestDto>>, GetMembershipRequestsQueryHdlr>();

        services.AddScoped<IPlatformAdminAuthorizer, PlatformAdminAuthorizer>();

        return services;
    }
}
