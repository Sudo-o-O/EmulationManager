namespace EmulationManager.Steam;

public sealed class SteamShortcut
{
    public required string ManagedId { get; init; }

    public required string Name { get; init; }

    public required string ExecutablePath { get; init; }

    public required string StartDirectory { get; init; }

    public required string LaunchArguments { get; init; }

    public string? IconPath { get; init; }

    public IReadOnlyList<string> Tags { get; init; } =
        [];

    public bool IsHidden { get; init; }

    public bool AllowDesktopConfig { get; init; } = true;

    public bool AllowOverlay { get; init; } = true;
}