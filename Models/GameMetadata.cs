namespace EmulationManager.Models;

public sealed class GameMetadata
{
    public string? TitleId { get; init; }

    public string? DisplayName { get; init; }

    public string? Publisher { get; init; }

    public ulong? Version { get; init; }

    public bool WasReadFromFile { get; init; }
}