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

    public IReadOnlyList<SteamGameEntry> Build(
        IReadOnlyCollection<GameFile> files)
    {
        return files
            .Where(IsLaunchableGame)
            .GroupBy(
                file => new
                {
                    file.ConsoleName,
                    file.GameName
                })
            .Select(group =>
            {
                GameFile game =
                    group.First();

                return new SteamGameEntry
                {
                    Id = gameIdService.CreateId(game),
                    Title = game.GameName,
                    ConsoleName = game.ConsoleName,
                    RomPath = game.FullPath,

                    ManagerExecutablePath =
                        managerExecutablePath
                };
            })
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