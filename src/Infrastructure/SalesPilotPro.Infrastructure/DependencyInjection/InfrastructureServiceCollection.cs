using Microsoft.Extensions.DependencyInjection;
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.Contexts;
using SalesPilotPro.Infrastructure.Security;

namespace SalesPilotPro.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Contextos (se setean desde middleware)
        services.AddScoped<ITenantContext>(_ => null!);
        services.AddScoped<IUserContext>(_ => null!);
        services.AddScoped<IModuleContext>(_ => null!);

        // JWT DEV
        services.AddScoped<IJwtProvider, JwtProviderDev>();

        return services;
    }
}
