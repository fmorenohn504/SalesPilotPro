using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Api.Controllers;

[ApiController]
[Route("api/secure")]
[Authorize] // 🔐 PROTEGIDO POR JWT
public sealed class SecureTestController : ControllerBase
{
    private readonly ITenantContext _tenant;
    private readonly IUserContext _user;
    private readonly IModuleContext _modules;

    public SecureTestController(
        ITenantContext tenant,
        IUserContext user,
        IModuleContext modules)
    {
        _tenant = tenant;
        _user = user;
        _modules = modules;
    }

    [HttpGet("whoami")]
public IActionResult WhoAmI()
{
    // Ejemplos de módulos a verificar (DEV)
    var checkedModules = new[] { "Budget", "Reports", "Users" };

    var moduleStatus = checkedModules
        .Select(m => new
        {
            module = m,
            enabled = _modules.IsModuleEnabled(m)
        });

    return Ok(new
    {
        tenantId = _tenant.TenantId,
        userId = _user.UserId,
        roles = _user.Roles,
        modules = moduleStatus
    });
}
}