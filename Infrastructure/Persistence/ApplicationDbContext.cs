using Core.Contracts;
using Core.Entity;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly string? _tenantId;
    private readonly ITenantService _tenantService;

    public DbSet<Product> Products { get; set; }

    public ApplicationDbContext(DbContextOptions options, ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
        _tenantId = _tenantService.GetTenant()?.Tid;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasQueryFilter(a => a.TenantId == _tenantId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var tenantConnectionString = _tenantService.GetConnectionString();
        if (!string.IsNullOrEmpty(tenantConnectionString))
        {
            var dbProvider = _tenantService.GetDatabaseProvider();
            if (dbProvider.ToLower() == "mssql")
            {
                optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    entry.Entity.TenantId = _tenantId;
                    break;
            }
        }
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}