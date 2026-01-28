namespace SalesPilotPro.Core.Contexts;

// Usuario autenticado (desde JWT)
public interface IUserContext
{
    Guid UserId { get; }
    IReadOnlyList<string> Roles { get; }
}
