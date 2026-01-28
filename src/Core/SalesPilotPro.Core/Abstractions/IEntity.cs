namespace SalesPilotPro.Core.Abstractions;

// Entidad base multi-tenant
public interface IEntity
{
    Guid Id { get; }
    Guid TenantId { get; }
}
