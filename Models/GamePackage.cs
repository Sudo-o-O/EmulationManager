namespace EmulationManager.Models;

public abstract class GamePackage
{
    public required string FilePath { get; init; }

    public required string FileName { get; init; }

    public long FileSize { get; init; }

    public DateTime LastModified { get; init; }

    public bool IsValid { get; init; }
}