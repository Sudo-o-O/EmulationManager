using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class GameLibraryService
{
    private readonly LibraryScanner libraryScanner;
    private readonly LibraryBuilder libraryBuilder;

    public GameLibraryService(
        LibraryScanner libraryScanner,
        LibraryBuilder libraryBuilder)
    {
        this.libraryScanner = libraryScanner;
        this.libraryBuilder = libraryBuilder;
    }

    public async Task<LibraryGame?> FindByIdAsync(
        string gameId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(gameId))
        {
            return null;
        }

        IReadOnlyList<GameFile> files =
            libraryScanner.Scan();

        IReadOnlyList<LibraryGame> games =
            await libraryBuilder.BuildAsync(
                files,
                cancellationToken);

        return games.FirstOrDefault(game =>
            game.Id.Equals(
                gameId,
                StringComparison.OrdinalIgnoreCase));
    }
}