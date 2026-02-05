using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPilotPro.Api.Controllers.Auth.Models;
using SalesPilotPro.Core.Security;
using SalesPilotPro.Infrastructure.Persistence;

namespace SalesPilotPro.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly SalesPilotProDbContext _db;
    private readonly IJwtProvider _jwtProvider;

    public AuthController(
        SalesPilotProDbContext db,
        IJwtProvider jwtProvider)
    {
        _db = db;
        _jwtProvider = jwtProvider;
    }

    [AllowAnonymous]
    [HttpPost("login-dev")]
    public async Task<IActionResult> LoginDev([FromBody] LoginDevRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required");

        var user = await _db.UsersTemp
            .IgnoreQueryFilters() // ⬅️ CLAVE
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user is null)
            return NotFound("User not found");

        var roles = new[] { "admin" };
        var modules = new[] { "CRM", "REPORTS", "ADMIN" };

        var token = _jwtProvider.GenerateToken(
            tenantId: user.TenantId,
            userId: user.Id,
            roles: roles,
            modules: modules
        );

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer"
        });
    }
}
