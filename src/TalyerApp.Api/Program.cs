using TalyerApp.Api.Configuration;
using TalyerApp.Api.DependencyInjection;
using TalyerApp.Api.Endpoints;
using TalyerApp.Api.Extensions;
using TalyerApp.Api.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.AddTalyerDevelopmentUserSecrets();

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration)
    .AddTalyerAuthentication(builder.Configuration)
    .AddTalyerOpenApi(builder.Configuration);

var app = builder.Build();

if (await app.TryRunCliCommandsAsync(args))
{
    return;
}

app.Services.GetRequiredService<ErrorCatalogValidator>().Validate();

app.MapTalyerDevelopmentApiDocs();
app.UseTalyerPipeline();
app.MapApiEndpoints();

app.Run();
