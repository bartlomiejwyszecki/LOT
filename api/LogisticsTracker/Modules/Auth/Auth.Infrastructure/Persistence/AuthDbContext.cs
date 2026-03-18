using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public const string SchemaName = "auth";

    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalProviderLink> ExternalProviderLinks => Set<ExternalProviderLink>();

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}