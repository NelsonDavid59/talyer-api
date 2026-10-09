using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TalyerApp.Application.Common.Dispatching;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Features.UserRoleAssignments;
using TalyerApp.Application.Features.MembershipRequests;
using TalyerApp.Application.Common.Authorization;
using TalyerApp.Application.Common.Interfaces.Identity;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Application.Common.Interfaces.Localization;
using TalyerApp.Api.Extensions;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Infrastructure.Identity;
using TalyerApp.Infrastructure.Notifications;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Persistence;
using TalyerApp.Application.Interfaces;
using TalyerApp.Api.Localization;
using TalyerApp.Infrastructure.Persistence;
using TalyerApp.Infrastructure.Persistence.Repositories;
using TalyerApp.Infrastructure.Persistence.Seeding;
using System.Security.Claims;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile(
        Path.Combine(builder.Environment.ContentRootPath, "usersecrets.json"),
        optional: true,
        reloadOnChange: false);
}

builder.Services.Configure<PlatformAdminBootstrapOptions>(
    builder.Configuration.GetSection(PlatformAdminBootstrapOptions.SectionName));
builder.Services.Configure<KeycloakAdminOptions>(
    builder.Configuration.GetSection(KeycloakAdminOptions.SectionName));
builder.Services.Configure<MembershipOptions>(
    builder.Configuration.GetSection(MembershipOptions.SectionName));

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the command and query dispatchers
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// Register the command and query handlers
builder.Services.AddScoped<ICommandHandler<RegisterExternalIdentityCmd, Guid>, RegisterExternalIdentityCmdHdlr>();
builder.Services.AddScoped<ICommandHandler<AssignUserRoleCmd, int>, AssignUserRoleCmdHdlr>();
builder.Services.AddScoped<ICommandHandler<SubmitMembershipRequestCmd, int>, SubmitMembershipRequestCmdHdlr>();
builder.Services.AddScoped<ICommandHandler<ConfirmMembershipRequestEmailCmd, int>, ConfirmMembershipRequestEmailCmdHdlr>();
builder.Services.AddScoped<ICommandHandler<ApproveMembershipRequestCmd, int>, ApproveMembershipRequestCmdHdlr>();
builder.Services.AddScoped<ICommandHandler<RejectMembershipRequestCmd, int>, RejectMembershipRequestCmdHdlr>();
builder.Services.AddScoped<IQueryHandler<GetMembershipRequestsQuery, IReadOnlyList<MembershipRequestDto>>, GetMembershipRequestsQueryHdlr>();

// Register repositories and unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRep, UserRep>();
builder.Services.AddScoped<IExternalIdentityRep, ExternalIdentityRep>();
builder.Services.AddScoped<IUserRoleAssignmentRep, UserRoleAssignmentRep>();
builder.Services.AddScoped<IRoleRep, RoleRep>();
builder.Services.AddScoped<ITenantRep, TenantRep>();
builder.Services.AddScoped<IBranchRep, BranchRep>();
builder.Services.AddScoped<IMembershipRequestRep, MembershipRequestRep>();
builder.Services.AddScoped<IPlatformAdminAuthorizer, PlatformAdminAuthorizer>();
builder.Services.AddScoped<IEmailSender, LoggingEmailSender>();
builder.Services.AddHttpClient<IIdentityAdminService, KeycloakIdentityAdminService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<KeycloakAdminOptions>>().Value;
    var baseUrl = string.IsNullOrWhiteSpace(options.BaseUrl)
        ? "http://localhost:8080"
        : options.BaseUrl.TrimEnd('/');
    client.BaseAddress = new Uri(baseUrl + "/");
});
builder.Services.AddSingleton<ErrorCatalog>();
builder.Services.AddSingleton<IErrorMessageResolver, JsonErrorMessageResolver>();
builder.Services.AddSingleton<IErrorHttpStatusMapper, ErrorHttpStatusMapper>();
builder.Services.AddSingleton<ErrorCatalogValidator>();
builder.Services.AddSingleton<IUniqueConstraintRegistry>(_ => UniqueConstraintRegistry.Create());
builder.Services.AddScoped<IDatabaseExceptionTranslator, PostgresDatabaseExceptionTranslator>();
builder.Services.AddScoped<RbacReferenceDataSeeder>();
builder.Services.AddScoped<PlatformTenantReferenceDataSeeder>();
builder.Services.AddScoped<IReferenceDataSeeder, ReferenceDataSeeder>();
builder.Services.AddScoped<IPlatformAdminBootstrapSeeder, PlatformAdminBootstrapSeeder>();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/talyer-realm";
        options.Audience = "talyer-api";
        options.RequireHttpsMetadata = false;
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                var providerUserId = principal?.FindFirst("sub")?.Value;
                var email = principal?.FindFirst("email")?.Value;
                var username = principal?.FindFirst("preferred_username")?.Value;
                var firstName = principal?.FindFirst("given_name")?.Value;
                var lastName = principal?.FindFirst("family_name")?.Value;

                if (string.IsNullOrWhiteSpace(providerUserId) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(lastName))
                {
                    Console.WriteLine("llegue");
                    context.Fail("The token does not contain the required user claims.");
                    return;
                }

                var dispatcher = context.HttpContext.RequestServices
                    .GetRequiredService<ICommandDispatcher>();

                var command = new RegisterExternalIdentityCmd(
                    Provider: "keycloak",
                    ProviderUserId: providerUserId,
                    Username: username,
                    FirstName: firstName,
                    LastName: lastName,
                    Email: email);

                var result = await dispatcher.DispatchAsync<RegisterExternalIdentityCmd, Guid>(
                    command,
                    context.HttpContext.RequestAborted);

                if (result.IsFailure)
                {
                    var messageResolver = context.HttpContext.RequestServices
                        .GetRequiredService<IErrorMessageResolver>();
                    context.Fail(messageResolver.ResolveMessage(result.Error));
                    return;
                }

                principal!.AddIdentity(new ClaimsIdentity(
                [
                    new Claim("talyer_user_id", result.Value.ToString())
                ]));
            }
        };
    });

// Add authorization services
builder.Services.AddAuthorization();


builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["OAuth2"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Keycloak OAuth2 Authorization Code + PKCE",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(
                            "http://localhost:8080/realms/talyer-realm/protocol/openid-connect/auth"),

                        TokenUrl = new Uri(
                            "http://localhost:8080/realms/talyer-realm/protocol/openid-connect/token"),

                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID",
                            ["profile"] = "User profile",
                            ["email"] = "User email"
                        }
                    }
                }
            };

        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (args.Contains("seed", StringComparer.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IReferenceDataSeeder>();
    await seeder.SeedAsync();
    return;
}

if (args.Contains("bootstrap-platform-admin", StringComparer.OrdinalIgnoreCase))
{
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var bootstrap = scope.ServiceProvider.GetRequiredService<IPlatformAdminBootstrapSeeder>();
        await bootstrap.BootstrapAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        Environment.Exit(1);
    }

    return;
}

app.Services.GetRequiredService<ErrorCatalogValidator>().Validate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/talyer-app-api", options =>
    {
        options
            .AddPreferredSecuritySchemes("OAuth2")
            .AddAuthorizationCodeFlow("OAuth2", flow =>
            {
                flow.ClientId = "talyer-api-docs";
                flow.Pkce = Pkce.Sha256;

                flow.AddBodyParameter(
                    "client_id",
                    "talyer-api-docs");

                flow.SelectedScopes = ["openid", "profile", "email"];
            });
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/users/me", async (HttpContext httpContext, IUserRep userRepository) =>
{
    var userIdValue = httpContext.User.FindFirstValue("talyer_user_id");

    if (!Guid.TryParse(userIdValue, out var userId))
    {
        return Results.Unauthorized();
    }

    var userResult = await userRepository.GetByIdAsync(userId);

    return userResult.IsSuccess
        ? Results.Ok(UserDto.FromEntity(userResult.Value))
        : Results.NotFound();
})
.RequireAuthorization()
.WithName("GetCurrentUser");

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
    var auth = await EnsurePlatformAdmin(httpContext, platformAdminAuthorizer);
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
    var auth = await EnsurePlatformAdmin(httpContext, platformAdminAuthorizer);
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
    var auth = await EnsurePlatformAdmin(httpContext, platformAdminAuthorizer);
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

app.Run();

static async Task<IResult?> EnsurePlatformAdmin(
    HttpContext httpContext,
    IPlatformAdminAuthorizer platformAdminAuthorizer)
{
    var userIdValue = httpContext.User.FindFirstValue("talyer_user_id");
    if (!Guid.TryParse(userIdValue, out var userId))
    {
        return Results.Unauthorized();
    }

    var authResult = await platformAdminAuthorizer.EnsureIsPlatformAdminAsync(userId, httpContext.RequestAborted);
    if (authResult.IsFailure)
    {
        var messages = httpContext.RequestServices.GetRequiredService<IErrorMessageResolver>();
        var statuses = httpContext.RequestServices.GetRequiredService<IErrorHttpStatusMapper>();
        return authResult.Error.ToProblemResult(messages, statuses);
    }

    return null;
}

