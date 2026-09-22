using TalyerApp.Domain.Entities;
using TalyerApp.Application.Interfaces.Repository;
using TalyerApp.Domain.Shared.Result;
using Microsoft.EntityFrameworkCore;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class UserRep : IUserRep
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRep(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<User>();
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        var users = await _dbSet.ToListAsync();

        return Result<IEnumerable<User>>.Success(users);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var user = await _dbSet.FindAsync(id);
        if (user is null)
        {
            return Result<User>.Failure(DomainErrors.User.UserNotFound);
        }

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return Result<User>.Failure(DomainErrors.User.UserNotFound);
        }

        return Result<User>.Success(user);
    }

    public void Add(User user)
    {
        _dbSet.Add(user);
    }
}