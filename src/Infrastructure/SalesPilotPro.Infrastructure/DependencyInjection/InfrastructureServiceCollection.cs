using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.Persistence;
using SalesPilotPro.Infrastructure.Security;

namespace SalesPilotPro.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SalesPilotProDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IJwtProvider, JwtProviderDev>();

        return services;
    }
}
