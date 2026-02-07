using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Contexts;

public sealed class HttpUserContext : IUserContext
{
    public Guid UserId { get; }
    public IReadOnlyList<string> Roles { get; }

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var sub = user.FindFirst("sub")?.Value;

            if (Guid.TryParse(sub, out var userId))
            {
                UserId = userId;

                Roles = user.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value)
                            .ToList()
                            .AsReadOnly();

                return;
            }
        }

        // Estado NO autenticado (sin romper DI)
        UserId = Guid.Empty;
        Roles = Array.Empty<string>();
    }
}
