using Microsoft.EntityFrameworkCore;
using TalyerApp.Application.Common.Interfaces.Repository;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Result;
using TalyerApp.Infrastructure.Persistence;

namespace TalyerApp.Ifrastructure.Persistence.Repositories;

public class ExternalIdentityRep : IExternalIdentityRep
{
    private readonly ApplicationDbContext _context;
    private DbSet<ExternalIdentity> _dbSet;

    public ExternalIdentityRep(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<ExternalIdentity>();
    }

    public void Add(ExternalIdentity externalIdentity)
    {
        _dbSet.Add(externalIdentity);
    }

    public async Task<Result<IEnumerable<ExternalIdentity>>> GetAllAsync()
    {
        var results = await _dbSet.ToListAsync();

        return Result<IEnumerable<ExternalIdentity>>.Success(results);
    }

    public async Task<Result<ExternalIdentity>> GetByProviderAndProviderUserIdAsync(string provider, string providerUserId)
    {
        var externalIdentity = await _dbSet.FirstOrDefaultAsync(ei => ei.Provider == provider 
                                                                    && ei.ProviderUserId == providerUserId);
        
        if (externalIdentity is null)
        {
            return Result<ExternalIdentity>.Failure(DomainErrors.ExternalIdentity.ExternalIdentityNotFound);
        }

        return Result<ExternalIdentity>.Success(externalIdentity);
    }
}