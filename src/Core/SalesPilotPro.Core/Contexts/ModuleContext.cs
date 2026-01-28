using SalesPilotPro.Core.Contexts;

namespace SalesPilotPro.Infrastructure.Contexts;

// Módulos habilitados por tenant
public sealed class ModuleContext : IModuleContext
{
    private readonly HashSet<string> _enabled;

    public ModuleContext(IEnumerable<string> enabledModules)
    {
        _enabled = enabledModules
            .Select(m => m.ToLowerInvariant())
            .ToHashSet();
    }

    public bool IsModuleEnabled(string moduleKey)
        => _enabled.Contains(moduleKey.ToLowerInvariant());
}
