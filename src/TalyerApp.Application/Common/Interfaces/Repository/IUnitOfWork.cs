namespace TalyerApp.Application.Common.Interfaces.Repository;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
}