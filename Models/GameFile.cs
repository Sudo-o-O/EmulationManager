namespace EmulationManager.Models;

public sealed class GameFile
{
    public required string GameName { get; init; }

    public required string ConsoleName { get; init; }

    public required string ContentType { get; init; }

    public required string FileName { get; init; }

    public required string FullPath { get; init; }

    public required long SizeBytes { get; init; }
}
