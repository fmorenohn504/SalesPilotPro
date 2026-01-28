namespace SalesPilotPro.Core.Contexts;

// Información del tenant activo
public interface ITenantContext
{
    Guid TenantId { get; }
    string TenantCode { get; }
}
