using EmulationManager.Models;

namespace EmulationManager.Switch;

public sealed class SwitchPackage : GamePackage
{
    public required SwitchPackageType PackageType
    {
        get;
        init;
    }
}