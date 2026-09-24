using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TalyerApp.Application.Common.Dispatching;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Dto;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Interfaces;
using TalyerApp.Infrastructure.Persistence;
using TalyerApp.Infrastructure.Persistence.Repositories;
using System.Security.Claims;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

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

// Register repositories and unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRep, UserRep>();
builder.Services.AddScoped<IExternalIdentityRep, ExternalIdentityRep>();

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
                    context.Fail(result.Error.Message);
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

app.Run();

