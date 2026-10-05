using Microsoft.EntityFrameworkCore;
using TalyerApp.Domain.Entities;

namespace TalyerApp.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }

    public DbSet<ExternalIdentity> ExternalIdentities { get; set; }

    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<Branch> Branches { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
}