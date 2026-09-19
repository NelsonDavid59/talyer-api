namespace TalyerApp.Application.Interfaces.Repository;

public interface IUnitOfWork : IDisposable
{
    IUserProfileRep UserProfileRepository { get; }

    Task<int> SaveChangesAsync();

    Task RollbackAsync();
}