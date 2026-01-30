using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace SalesPilotPro.Api.Controllers.Base;

[Authorize]
[ApiController]
[EnableRateLimiting("fixed")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid UserId =>
        Guid.Parse(User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Missing sub claim"));

    protected Guid TenantId =>
        Guid.Parse(User.FindFirstValue("tid")
            ?? throw new UnauthorizedAccessException("Missing tid claim"));

    protected Guid SessionId =>
        Guid.Parse(User.FindFirstValue("sid")
            ?? throw new UnauthorizedAccessException("Missing sid claim"));
}
