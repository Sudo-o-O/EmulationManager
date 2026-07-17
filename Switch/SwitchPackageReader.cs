using EmulationManager.Models;
using EmulationManager.Services;

namespace EmulationManager.Switch;

public sealed class SwitchPackageReader
    : IPackageReader<SwitchPackage>
{
    private readonly SwitchPackageValidator validator;

    public SwitchPackageReader(
        SwitchPackageValidator validator)
    {
        this.validator = validator;
    }

    public bool CanRead(
        GameFile game)
    {
        ArgumentNullException.ThrowIfNull(game);

        if (!game.ConsoleName.Equals(
                "Switch",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string extension =
            Path.GetExtension(game.FullPath);

        return extension.Equals(
                   ".nsp",
                   StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(
                   ".xci",
                   StringComparison.OrdinalIgnoreCase);
    }

    public SwitchPackage Open(
        GameFile game)
    {
        ArgumentNullException.ThrowIfNull(game);

        if (!File.Exists(game.FullPath))
        {
            throw new FileNotFoundException(
                "The Nintendo Switch package was not found.",
                game.FullPath);
        }

        string extension =
            Path.GetExtension(game.FullPath);

        SwitchPackageType type =
            extension.ToLowerInvariant() switch
            {
                ".nsp" => SwitchPackageType.Nsp,
                ".xci" => SwitchPackageType.Xci,
                _ => SwitchPackageType.Unknown
            };

        FileInfo info =
            new(game.FullPath);

        SwitchPackage package = new()
        {
            FilePath = game.FullPath,
            FileName = game.FileName,
            PackageType = type,
            FileSize = info.Length,
            LastModified = info.LastWriteTimeUtc,
            IsValid = false
        };

        return validator.Validate(package);
    }

    public PackageInspectionResult<SwitchPackage> Inspect(
        GameFile game)
    {
        ArgumentNullException.ThrowIfNull(game);

        SwitchPackage package;

        try
        {
            package = Open(game);
        }
        catch (Exception exception)
        {
            package = CreateFailedPackage(game);

            return PackageInspectionResult<SwitchPackage>.Failed(
                package,
                exception.Message,
                exception);
        }

        if (!package.IsValid)
        {
            return PackageInspectionResult<SwitchPackage>.Failed(
                package,
                "The file is not a valid supported Nintendo Switch package.");
        }

        return PackageInspectionResult<SwitchPackage>.Succeeded(
            package);
    }

    private static SwitchPackage CreateFailedPackage(
        GameFile game)
    {
        string extension =
            Path.GetExtension(game.FullPath);

        SwitchPackageType packageType =
            extension.ToLowerInvariant() switch
            {
                ".nsp" => SwitchPackageType.Nsp,
                ".xci" => SwitchPackageType.Xci,
                _ => SwitchPackageType.Unknown
            };

        return new SwitchPackage
        {
            FilePath = game.FullPath,
            FileName = game.FileName,
            PackageType = packageType,
            FileSize = 0,
            LastModified = DateTime.MinValue,
            IsValid = false
        };
    }
}