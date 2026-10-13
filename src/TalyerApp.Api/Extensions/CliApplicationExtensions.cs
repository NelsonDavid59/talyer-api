using TalyerApp.Application.Common.Interfaces.Persistence;

namespace TalyerApp.Api.Extensions;

public static class CliApplicationExtensions
{
    public static async Task<bool> TryRunCliCommandsAsync(this WebApplication app, string[] args)
    {
        if (args.Contains("seed", StringComparer.OrdinalIgnoreCase))
        {
            await using var scope = app.Services.CreateAsyncScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IReferenceDataSeeder>();
            await seeder.SeedAsync();
            return true;
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

            return true;
        }

        return false;
    }
}
