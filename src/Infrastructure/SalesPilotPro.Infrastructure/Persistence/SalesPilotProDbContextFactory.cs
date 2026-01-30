using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Infrastructure.Persistence;

internal sealed class DesignTimeTenantContext : ITenantContext
{
    public Guid TenantId => Guid.Empty;
}

public sealed class SalesPilotProDbContextFactory
    : IDesignTimeDbContextFactory<SalesPilotProDbContext>
{
    public SalesPilotProDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var optionsBuilder =
            new DbContextOptionsBuilder<SalesPilotProDbContext>();

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"));

        var tenantContext = new DesignTimeTenantContext();

        return new SalesPilotProDbContext(optionsBuilder.Options, tenantContext);
    }
}
