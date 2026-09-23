using TalyerApp.Domain.Entities;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Shared.Result;
using Microsoft.EntityFrameworkCore;

namespace TalyerApp.Infrastructure.Persistence.Repositories;

public class UserRep : IUserRep
{
    private readonly ApplicationDbContext _context;

    public UserRep(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();

        return Result<IEnumerable<User>>.Success(users);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return Result<User>.Failure(DomainErrors.User.UserNotFound);
        }

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return Result<User>.Failure(DomainErrors.User.UserNotFound);
        }

        return Result<User>.Success(user);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}