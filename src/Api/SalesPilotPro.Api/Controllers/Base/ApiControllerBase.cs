using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPilotPro.Api.Controllers.Base;

namespace SalesPilotPro.Api.Controllers.Base;

[Authorize]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
}
