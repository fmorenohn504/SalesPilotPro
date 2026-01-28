using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Infrastructure.Contexts;

// Usuario autenticado (desde JWT)
public sealed class UserContext : IUserContext
{
    public Guid UserId { get; }
    public IReadOnlyList<string> Roles { get; }

    public UserContext(Guid userId, IEnumerable<string> roles)
    {
        UserId = userId;
        Roles = roles.ToList().AsReadOnly();
    }
}
