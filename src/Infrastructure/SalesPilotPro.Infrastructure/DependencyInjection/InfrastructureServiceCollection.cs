using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.Contexts;
using SalesPilotPro.Infrastructure.Persistence;
using SalesPilotPro.Infrastructure.Security;

namespace SalesPilotPro.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollection
{
    // Se amplía para recibir IConfiguration (sin acoplar API)
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Contextos (se setean desde middleware)
        services.AddScoped<ITenantContext>(_ => null!);
        services.AddScoped<IUserContext>(_ => null!);
        services.AddScoped<IModuleContext>(_ => null!);

        // DbContext (SQL Server – tenant-aware)
        services.AddDbContext<SalesPilotProDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // JWT DEV
        services.AddScoped<IJwtProvider, JwtProviderDev>();

        return services;
    }
}
