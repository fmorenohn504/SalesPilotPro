using Microsoft.AspNetCore.Mvc;
using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Auth;

// Login DEV – NO producción
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtProvider _jwtProvider;

    public AuthController(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // ⚠️ DEV ONLY – usuario hardcodeado
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username requerido");

        // Datos simulados (DEV)
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userId = Guid.NewGuid();

        var roles = new[] { "Admin" };
        var modules = new[] { "Budget", "Reports" };

        var token = _jwtProvider.GenerateToken(
            tenantId,
            userId,
            roles,
            modules);

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer"
        });
    }
}
