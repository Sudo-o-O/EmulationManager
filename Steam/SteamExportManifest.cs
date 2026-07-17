namespace EmulationManager.Steam;

public sealed class SteamExportManifest
{
    public DateTime GeneratedUtc { get; init; } =
        DateTime.UtcNow;

    public IReadOnlyList<SteamShortcut> Shortcuts
    {
        get;
        init;
    } = [];
}