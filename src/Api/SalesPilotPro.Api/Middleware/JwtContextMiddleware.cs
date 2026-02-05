using System.Security.Claims;
using SalesPilotPro.Core.Contexts;
using SalesPilotPro.Infrastructure.Contexts;

namespace SalesPilotPro.Api.Middleware;

// Middleware que construye los Contexts desde el JWT
public sealed class JwtContextMiddleware
{
    private readonly RequestDelegate _next;

    public JwtContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Si no hay usuario autenticado, continuamos (endpoints públicos)
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // Claims básicos
        var tenantIdClaim = context.User.FindFirst("tenantId");
        var userIdClaim = context.User.FindFirst("userId");

        if (tenantIdClaim == null || userIdClaim == null)
        {
            // JWT inválido para nuestro sistema
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var tenantId = Guid.Parse(tenantIdClaim.Value);
        var userId = Guid.Parse(userIdClaim.Value);

        // Roles
        var roles = context.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Módulos habilitados
        var modules = context.User
            .FindAll("module")
            .Select(c => c.Value)
            .ToList();

        // Crear contextos
        var tenantContext = new TenantContext(tenantId);
        var userContext = new UserContext(userId, roles);
        var moduleContext = new ModuleContext(modules);

        // Inyectarlos en RequestServices (scoped)
        context.RequestServices.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider
            .GetService<ITenantContext>();

        context.Items[nameof(ITenantContext)] = tenantContext;
        context.Items[nameof(IUserContext)] = userContext;
        context.Items[nameof(IModuleContext)] = moduleContext;

        await _next(context);
    }
}
