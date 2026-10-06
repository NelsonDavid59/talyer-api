namespace TalyerApp.Application.Common.Interfaces.Persistence;

public interface IReferenceDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
