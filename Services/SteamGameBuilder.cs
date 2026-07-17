using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class SteamGameBuilder
{
    private readonly GameIdService gameIdService;
    private readonly string managerExecutablePath;

    public SteamGameBuilder(
        GameIdService gameIdService)
    {
        this.gameIdService = gameIdService;

        managerExecutablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "The Emulation Manager executable path " +
                "could not be determined.");
    }

    public async Task<IReadOnlyList<SteamGameEntry>> BuildAsync(
        IReadOnlyCollection<GameFile> files,
        CancellationToken cancellationToken = default)
    {
        var results = new List<SteamGameEntry>();

        IEnumerable<IGrouping<
            (string ConsoleName, string GameName),
            GameFile>> groups =
            files
                .Where(IsLaunchableGame)
                .GroupBy(file =>
                    (
                        file.ConsoleName,
                        file.GameName
                    ));

        foreach (var group in groups)
        {
            cancellationToken.ThrowIfCancellationRequested();

            GameFile game = group.First();

            string gameId =
                await gameIdService.CreateIdAsync(
                    game,
                    cancellationToken);

            results.Add(
                new SteamGameEntry
                {
                    Id = gameId,
                    Title = game.GameName,
                    ConsoleName = game.ConsoleName,
                    RomPath = game.FullPath,

                    ManagerExecutablePath =
                        managerExecutablePath
                });
        }

        return results
            .OrderBy(game => game.ConsoleName)
            .ThenBy(game => game.Title)
            .ToList();
    }

    private static bool IsLaunchableGame(
        GameFile game)
    {
        return game.ContentType.Equals(
                   "Base Game",
                   StringComparison.OrdinalIgnoreCase) ||
               game.ContentType.Equals(
                   "Game",
                   StringComparison.OrdinalIgnoreCase);
    }
}