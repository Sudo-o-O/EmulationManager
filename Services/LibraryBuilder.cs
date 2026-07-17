using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class LibraryBuilder
{
    private readonly GameIdService gameIdService;

    public LibraryBuilder(
        GameIdService gameIdService)
    {
        this.gameIdService = gameIdService;
    }

    public async Task<IReadOnlyList<LibraryGame>> BuildAsync(
        IReadOnlyCollection<GameFile> files,
        CancellationToken cancellationToken = default)
    {
        var results = new List<LibraryGame>();

        var groups =
            files.GroupBy(
                file => new
                {
                    file.ConsoleName,
                    file.GameName
                });

        foreach (var group in groups)
        {
            cancellationToken.ThrowIfCancellationRequested();

            GameFile? baseGame =
                group.FirstOrDefault(file =>
                    IsBaseGame(file.ContentType));

            if (baseGame is null)
            {
                continue;
            }

            string id =
                await gameIdService.CreateIdAsync(
                    baseGame,
                    cancellationToken);

            results.Add(
                new LibraryGame
                {
                    Id = id,
                    Title = baseGame.GameName,
                    ConsoleName = baseGame.ConsoleName,
                    BaseGame = baseGame,

                    Updates = group
                        .Where(file =>
                            file.ContentType.Equals(
                                "Update",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderBy(file => file.FileName)
                        .ToList(),

                    Dlc = group
                        .Where(file =>
                            file.ContentType.Equals(
                                "DLC",
                                StringComparison.OrdinalIgnoreCase))
                        .OrderBy(file => file.FileName)
                        .ToList()
                });
        }

        return results
            .OrderBy(game => game.ConsoleName)
            .ThenBy(game => game.Title)
            .ToList();
    }

    private static bool IsBaseGame(
        string contentType)
    {
        return contentType.Equals(
                   "Base Game",
                   StringComparison.OrdinalIgnoreCase) ||
               contentType.Equals(
                   "Game",
                   StringComparison.OrdinalIgnoreCase);
    }
}