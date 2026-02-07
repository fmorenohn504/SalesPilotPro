using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Controllers;

[ApiController]
[Route("api/secure")]
public sealed class SecureTestController : ControllerBase
{
    private readonly ITenantContext _tenantContext;
    private readonly IUserContext _userContext;
    private readonly IModuleContext _moduleContext;

    public SecureTestController(
        ITenantContext tenantContext,
        IUserContext userContext,
        IModuleContext moduleContext)
    {
        _tenantContext = tenantContext;
        _userContext = userContext;
        _moduleContext = moduleContext;
    }

    // ⚠️ SOLO PARA DEV / JWT TEST
    [AllowAnonymous]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            tenantId = _tenantContext.TenantId,
            tenantCode = _tenantContext.TenantCode,
            userId = _userContext.UserId,
            roles = _userContext.Roles,
            crmEnabled = _moduleContext.IsModuleEnabled("CRM")
        });
    }
}
