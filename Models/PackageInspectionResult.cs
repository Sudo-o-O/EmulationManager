namespace EmulationManager.Models;

public sealed class PackageInspectionResult<TPackage>
    where TPackage : GamePackage
{
    public required TPackage Package { get; init; }

    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public Exception? Exception { get; init; }

    public static PackageInspectionResult<TPackage> Succeeded(
        TPackage package)
    {
        return new PackageInspectionResult<TPackage>
        {
            Package = package,
            Success = true
        };
    }

    public static PackageInspectionResult<TPackage> Failed(
        TPackage package,
        string errorMessage,
        Exception? exception = null)
    {
        return new PackageInspectionResult<TPackage>
        {
            Package = package,
            Success = false,
            ErrorMessage = errorMessage,
            Exception = exception
        };
    }
}
