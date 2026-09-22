using Microsoft.EntityFrameworkCore;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    
}