using Microsoft.AspNetCore.Mvc;

namespace SalesPilotPro.Api.Controllers;

// Endpoint simple para validar que la API está viva
[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "OK",
            service = "SalesPilotPro API"
        });
    }
}
