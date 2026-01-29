using SalesPilotPro.Infrastructure.Entities;

namespace SalesPilotPro.Infrastructure.Entities.Auth;

// Usuario temporal SOLO para DEV
public sealed class UserTemp : TenantEntity
{
    public Guid Id { get; set; }

    // Username usado en login DEV
    public string Username { get; set; } = string.Empty;

    // Flags simples para pruebas
    public bool IsActive { get; set; } = true;
}
