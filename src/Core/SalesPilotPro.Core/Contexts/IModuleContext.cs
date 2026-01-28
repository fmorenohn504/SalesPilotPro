namespace SalesPilotPro.Core.Contexts;

// Módulos habilitados por tenant
public interface IModuleContext
{
    bool IsModuleEnabled(string moduleKey);
}
