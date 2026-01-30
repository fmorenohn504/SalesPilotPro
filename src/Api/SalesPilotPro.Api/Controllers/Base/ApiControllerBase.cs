using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace SalesPilotPro.Api.Controllers.Base;

[Authorize]
[ApiController]
[EnableRateLimiting("fixed")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}
