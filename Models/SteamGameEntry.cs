namespace EmulationManager.Models;

public sealed class SteamGameEntry
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public required string ConsoleName { get; init; }

    public required string RomPath { get; init; }

    public required string ManagerExecutablePath { get; init; }

    public string LaunchArguments =>
        $"--launch \"{RomPath}\"";
}
