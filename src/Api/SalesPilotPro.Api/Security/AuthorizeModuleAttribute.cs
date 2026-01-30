using Microsoft.AspNetCore.Authorization;

namespace SalesPilotPro.Api.Security;

public sealed class AuthorizeModuleAttribute : AuthorizeAttribute
{
    public AuthorizeModuleAttribute(string requiredModule = "CRM")
    {
        Policy = "MODULE_CRM";
    }
}
