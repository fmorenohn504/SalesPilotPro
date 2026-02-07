using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SalesPilotPro.Api.Contexts;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Middleware;

public sealed class JwtContextMiddleware
{
    private readonly RequestDelegate _next;

    public JwtContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // ===== TENANT =====
            var tenantIdClaim = context.User.FindFirst("tid")?.Value;
            var tenantCodeClaim = context.User.FindFirst("tcode")?.Value ?? "DEV";

            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                var tenantContext = context.RequestServices.GetRequiredService<ITenantContext>();

                if (tenantContext is HttpTenantContext httpTenantContext)
                {
                    httpTenantContext.SetTenant(tenantId, tenantCodeClaim);
                }
            }

            // ===== USER =====
            var userIdClaim = context.User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var roles = context.User.FindAll("role")
                                        .Select(c => c.Value)
                                        .ToList();

                context.Items[nameof(IUserContext)] =
                    new UserContext(userId, roles);
            }

            // ===== MODULES =====
            var modules = context.User.FindAll("mods")
                                      .Select(c => c.Value)
                                      .ToArray();

            context.Items[nameof(IModuleContext)] =
                new ModuleContext(modules);
        }

        await _next(context);
    }
}

// ===== Implementaciones internas (como antes) =====

internal sealed class UserContext : IUserContext
{
    public Guid UserId { get; }

    // 🔴 CAMBIO CLAVE: IReadOnlyList (no Collection)
    public IReadOnlyList<string> Roles { get; }

    public UserContext(Guid userId, IEnumerable<string> roles)
    {
        UserId = userId;
        Roles = roles.ToList();
    }
}

internal sealed class ModuleContext : IModuleContext
{
    private readonly HashSet<string> _modules;

    public ModuleContext(IEnumerable<string> modules)
    {
        _modules = new HashSet<string>(modules, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsModuleEnabled(string module) => _modules.Contains(module);
}
