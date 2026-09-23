using Scalar.AspNetCore;
using TalyerApp.Application.Common.Dispatching;
using TalyerApp.Application.Common.Interfaces.CQRS;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Application.Features.ExternalIdentities;
using TalyerApp.Application.Interfaces;
using TalyerApp.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register the command and query dispatchers
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// Register the command and query handlers
builder.Services.AddScoped<ICommandHandler<RegisterExternalIdentityCmd, Guid>, RegisterExternalIdentityCmdHdlr>();

// Register repositories and unit of work
builder.Services.AddScoped<IUserRep, UserRep>();
builder.Services.AddScoped<IExternalIdentityRep, ExternalIdentityRep>();


// Add authentication services
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/talyer-realm";
        options.Audience = "talyer-api";
    });

// Add authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/talyer-app-api");
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
