using EmulationManager.Models;

namespace EmulationManager.Steam;

public sealed class SteamExportManifest
{
    public DateTime GeneratedUtc { get; init; } =
        DateTime.UtcNow;

    public List<SteamGameEntry> Games { get; init; } = [];
}
