using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Infrastructure.Entities;
using SalesPilotPro.Infrastructure.Entities.Crm;

namespace SalesPilotPro.Infrastructure.Persistence;

public sealed class SalesPilotProDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public Guid TenantId => _tenantContext.TenantId;

    public SalesPilotProDbContext(
        DbContextOptions<SalesPilotProDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    // =======================
    // DbSets
    // =======================

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Tenant Filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateTenantFilter(entityType.ClrType));
            }
        }

        // AuditEvents mapping
        modelBuilder.Entity<AuditEvent>(b =>
        {
            b.ToTable("AuditEvents", "crm");
            b.HasKey(x => x.Id);
        });
    }

    private LambdaExpression CreateTenantFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(TenantEntity.TenantId));
        var tenantId = Expression.Property(Expression.Constant(this), nameof(TenantId));
        var body = Expression.Equal(property, tenantId);

        return Expression.Lambda(body, parameter);
    }
}
