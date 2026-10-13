namespace TalyerApp.Application.Common.Interfaces.Persistence;

public interface IPlatformAdminBootstrapSeeder
{
    Task BootstrapAsync(CancellationToken cancellationToken = default);
}
