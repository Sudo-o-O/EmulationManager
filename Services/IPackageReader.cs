using EmulationManager.Models;

namespace EmulationManager.Services;

public interface IPackageReader<TPackage>
    where TPackage : GamePackage
{
    bool CanRead(GameFile game);

    TPackage Open(GameFile game);

    PackageInspectionResult<TPackage> Inspect(
        GameFile game);
}