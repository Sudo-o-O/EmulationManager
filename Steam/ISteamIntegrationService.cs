namespace EmulationManager.Steam;

public interface ISteamIntegrationService
{
    Task ExportAsync(
        IReadOnlyCollection<SteamShortcut> shortcuts,
        CancellationToken cancellationToken = default);
}