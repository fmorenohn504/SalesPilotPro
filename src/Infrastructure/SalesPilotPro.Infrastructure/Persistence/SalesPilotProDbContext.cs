using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Infrastructure.Entities;
using SalesPilotPro.Infrastructure.Entities.Auth;
using SalesPilotPro.Infrastructure.Entities.Crm;

namespace SalesPilotPro.Infrastructure.Persistence;

// DbContext base del sistema (multi-tenant enforced)
public sealed class SalesPilotProDbContext : DbContext
{
    // 🔐 Tenant dinámico (seteado por middleware)
    public Guid TenantId { get; set; }

    public SalesPilotProDbContext(
        DbContextOptions<SalesPilotProDbContext> options)
        : base(options)
    {
    }

    // 🔹 DbSet DEV
    public DbSet<UserTemp> UsersTemp => Set<UserTemp>();

    // 🔹 DbSet CRM
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔒 Filtro global por Tenant (OWASP A01)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(
                        CreateTenantFilter(entityType.ClrType));
            }
        }
    }

    // entity => entity.TenantId == this.TenantId
    private LambdaExpression CreateTenantFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(TenantEntity.TenantId));
        var tenantId = Expression.Property(Expression.Constant(this), nameof(TenantId));
        var body = Expression.Equal(property, tenantId);

        return Expression.Lambda(body, parameter);
    }
}
