namespace SalesPilotPro.Core.Security;

// Contrato del proveedor de JWT (reemplazable)
public interface IJwtProvider
{
    string GenerateToken(
        Guid tenantId,
        Guid userId,
        IEnumerable<string> roles,
        IEnumerable<string> modules);
}
