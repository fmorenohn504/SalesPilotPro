using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPilotPro.Api.Controllers.Base;
using SalesPilotPro.Api.Security;

namespace SalesPilotPro.Api.Controllers.Crm;

[ApiVersion("1.0")]
[Authorize]
[AuthorizeModule("crm")]
[Route("api/crm")]
public sealed class CrmController : ApiControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "CRM module OK"
        });
    }
}
