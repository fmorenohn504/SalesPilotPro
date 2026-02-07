using Microsoft.AspNetCore.Http;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpModuleContext : IModuleContext
{
    private readonly HashSet<string> _modules;

    public HttpModuleContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            _modules = user.FindAll("mods")
                           .Select(c => c.Value)
                           .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            // Estado no autenticado → sin módulos
            _modules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool IsModuleEnabled(string module)
        => _modules.Contains(module);
}
