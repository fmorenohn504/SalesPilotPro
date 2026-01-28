using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Infrastructure.Security;

// Implementación JWT SOLO para desarrollo
public sealed class JwtProviderDev : IJwtProvider
{
    // ⚠️ Clave SOLO DEV (no usar en producción)
    private const string SecretKey = "DEV_ONLY_SECRET_KEY_CHANGE_LATER";

    public string GenerateToken(
        Guid tenantId,
        Guid userId,
        IEnumerable<string> roles,
        IEnumerable<string> modules)
    {
        var claims = new List<Claim>
        {
            new("tenantId", tenantId.ToString()),
            new("userId", userId.ToString())
        };

        // Roles
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        // Módulos habilitados
        claims.AddRange(modules.Select(m => new Claim("module", m)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
