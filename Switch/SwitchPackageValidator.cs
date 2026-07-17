namespace EmulationManager.Switch;

public sealed class SwitchPackageValidator
{
    public SwitchPackage Validate(
        SwitchPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);

        if (!File.Exists(package.FilePath))
        {
            return CopyWithValidity(
                package,
                isValid: false);
        }

        if (package.PackageType ==
            SwitchPackageType.Unknown)
        {
            return CopyWithValidity(
                package,
                isValid: false);
        }

        if (package.FileSize <= 0)
        {
            return CopyWithValidity(
                package,
                isValid: false);
        }

        return CopyWithValidity(
            package,
            isValid: true);
    }

    private static SwitchPackage CopyWithValidity(
        SwitchPackage package,
        bool isValid)
    {
        return new SwitchPackage
        {
            FilePath = package.FilePath,
            FileName = package.FileName,
            PackageType = package.PackageType,
            FileSize = package.FileSize,
            LastModified = package.LastModified,
            IsValid = isValid
        };
    }
}