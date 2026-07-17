using EmulationManager.Models;
using EmulationManager.Steam;

namespace EmulationManager.Services;

public sealed class SteamGameBuilder
{
    private readonly string managerExecutablePath;
    private readonly string managerWorkingDirectory;

    public SteamGameBuilder()
    {
        managerExecutablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "The Emulation Manager executable path " +
                "could not be determined.");

        managerWorkingDirectory =
            Path.GetDirectoryName(managerExecutablePath)
            ?? throw new InvalidOperationException(
                "The Emulation Manager working directory " +
                "could not be determined.");
    }

    public IReadOnlyList<SteamShortcut> Build(
        IReadOnlyCollection<LibraryGame> games)
    {
        return games
            .Select(game =>
                new SteamShortcut
                {
                    ManagedId = game.Id,
                    Name = game.Title,

                    ExecutablePath =
                        managerExecutablePath,

                    StartDirectory =
                        managerWorkingDirectory,

                    LaunchArguments =
                        $"--game-id \"{game.Id}\"",

                    Tags =
                    [
                        "EmulationManager",
                        game.ConsoleName
                    ],

                    AllowDesktopConfig = true,
                    AllowOverlay = true
                })
            .OrderBy(shortcut => shortcut.Name)
            .ToList();
    }
}