using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class SteamGameBuilder
{
    private readonly string managerExecutablePath;

    public SteamGameBuilder()
    {
        managerExecutablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "The Emulation Manager executable path " +
                "could not be determined.");
    }

    public IReadOnlyList<SteamGameEntry> Build(
        IReadOnlyCollection<LibraryGame> games)
    {
        return games
            .Select(game =>
                new SteamGameEntry
                {
                    Id = game.Id,
                    Title = game.Title,
                    ConsoleName = game.ConsoleName,
                    RomPath = game.BaseGame.FullPath,

                    ManagerExecutablePath =
                        managerExecutablePath
                })
            .OrderBy(game => game.ConsoleName)
            .ThenBy(game => game.Title)
            .ToList();
    }
}