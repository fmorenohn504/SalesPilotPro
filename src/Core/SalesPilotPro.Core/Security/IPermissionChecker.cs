namespace SalesPilotPro.Core.Security;

// Evaluación de permisos (por módulo)
public interface IPermissionChecker
{
    bool HasAccess(string permission);
}
