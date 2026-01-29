namespace SalesPilotPro.Infrastructure.Entities;

// Entidad base para TODAS las tablas tenant-aware
public abstract class TenantEntity
{
    // 🔒 Aislamiento por tenant (OWASP A01)
    public Guid TenantId { get; set; }
}
