using Microsoft.AspNetCore.Authorization;

namespace SalesPilotPro.Api.Security;

public sealed class AuthorizeModuleAttribute : AuthorizeAttribute
{
    public AuthorizeModuleAttribute(string module)
    {
        Policy = $"MODULE_{module}";
    }
}
