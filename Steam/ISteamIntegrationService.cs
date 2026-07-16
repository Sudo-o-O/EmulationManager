using EmulationManager.Models;

namespace EmulationManager.Steam;

public interface ISteamIntegrationService
{
    string Name { get; }

    bool IsAvailable { get; }

    Task ExportAsync(
        IReadOnlyCollection<SteamGameEntry> games,
        CancellationToken cancellationToken = default);
}
