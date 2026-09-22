namespace TalyerApp.Application.Interfaces.Repository;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
}