using EmulationManager.Models;
using EmulationManager.Switch;

namespace EmulationManager.Metadata;

public sealed class SwitchMetadataProvider
    : IGameMetadataProvider
{
    private readonly SwitchPackageReader packageReader;
    private readonly SwitchKeyLoader keyLoader;
    private readonly string prodKeysPath;

    public SwitchMetadataProvider(
        SwitchPackageReader packageReader,
        SwitchKeyLoader keyLoader,
        string prodKeysPath)
    {
        this.packageReader = packageReader;
        this.keyLoader = keyLoader;
        this.prodKeysPath = prodKeysPath;
    }

    public bool CanRead(GameFile game)
    {
        return packageReader.CanRead(game);
    }

    public Task<GameMetadata?> ReadAsync(
        GameFile game,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        PackageInspectionResult<SwitchPackage> inspection =
            packageReader.Inspect(game);

        if (!inspection.Success)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Switch package inspection failed: " +
                inspection.ErrorMessage);

            return Task.FromResult<GameMetadata?>(null);
        }

        if (string.IsNullOrWhiteSpace(prodKeysPath) ||
            !File.Exists(prodKeysPath))
        {
            System.Diagnostics.Debug.WriteLine(
                "Switch metadata could not be read because " +
                "prod.keys is not configured.");

            return Task.FromResult<GameMetadata?>(null);
        }

        try
        {
            SwitchKeySet keys =
                keyLoader.Load(prodKeysPath);

            System.Diagnostics.Debug.WriteLine(
                $"Switch package and keys validated. " +
                $"Loaded {keys.Count} key entries.");

            // Internal package metadata extraction will be added here.
            // Returning null allows the filename provider to run next.
            return Task.FromResult<GameMetadata?>(null);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Switch keys could not be loaded: " +
                exception.Message);

            return Task.FromResult<GameMetadata?>(null);
        }
    }
}