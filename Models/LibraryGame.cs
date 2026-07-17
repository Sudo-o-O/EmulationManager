namespace EmulationManager.Models;

public sealed class LibraryGame
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public required string ConsoleName { get; init; }

    public required GameFile BaseGame { get; init; }

    public IReadOnlyList<GameFile> Updates { get; init; } =
        [];

    public IReadOnlyList<GameFile> Dlc { get; init; } =
        [];

    public GameMetadata? Metadata { get; set; }

    public string? ArtworkPath { get; set; }

    public bool Favorite { get; set; }

    public DateTime? LastPlayed { get; set; }
}