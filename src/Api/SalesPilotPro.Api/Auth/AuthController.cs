using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Auth;

// Login DEV – NO producción
[ApiVersion("1.0")]
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtProvider _jwtProvider;
    private readonly bool _enableDevLogin;

    public AuthController(
        IJwtProvider jwtProvider,
        IConfiguration configuration)
    {
        _jwtProvider = jwtProvider;
        _enableDevLogin = configuration.GetValue<bool>("Features:EnableDevLogin");
    }

    [HttpPost("login-dev")]
    public IActionResult LoginDev([FromBody] LoginRequest request)
    {
        if (!_enableDevLogin)
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username requerido");

        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userId = Guid.NewGuid();

        var roles = new[] { "admin" };
        var modules = new[] { "crm", "reports", "admin" };

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
