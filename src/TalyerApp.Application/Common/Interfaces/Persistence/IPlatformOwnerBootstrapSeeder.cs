namespace TalyerApp.Application.Common.Interfaces.Persistence;

public interface IPlatformOwnerBootstrapSeeder
{
    Task BootstrapAsync(CancellationToken cancellationToken = default);
}
