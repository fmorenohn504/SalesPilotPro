using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SalesPilotPro.Infrastructure.Persistence;

// Factory SOLO para EF Core Tools (design-time)
public sealed class SalesPilotProDbContextFactory
    : IDesignTimeDbContextFactory<SalesPilotProDbContext>
{
    public SalesPilotProDbContext CreateDbContext(string[] args)
    {
        // EF se ejecuta desde el startup-project (API)
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();

        var optionsBuilder =
            new DbContextOptionsBuilder<SalesPilotProDbContext>();

        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"));

        // ⚠️ IMPORTANTE:
        // En design-time NO hay tenant real.
        // El TenantId se setea en runtime por middleware.
        return new SalesPilotProDbContext(optionsBuilder.Options);
    }
}
