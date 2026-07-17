using EmulationManager.Models;
using EmulationManager.Steam;

namespace EmulationManager.Services;

public sealed class SteamExportService
{
    private readonly LibraryScanner libraryScanner;
    private readonly LibraryBuilder libraryBuilder;
    private readonly SteamGameBuilder steamGameBuilder;
    private readonly ISteamIntegrationService steamIntegrationService;

    public SteamExportService(
        LibraryScanner libraryScanner,
        LibraryBuilder libraryBuilder,
        SteamGameBuilder steamGameBuilder,
        ISteamIntegrationService steamIntegrationService)
    {
        this.libraryScanner = libraryScanner;
        this.libraryBuilder = libraryBuilder;
        this.steamGameBuilder = steamGameBuilder;
        this.steamIntegrationService = steamIntegrationService;
    }

    public async Task<int> ExportAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<GameFile> files =
            libraryScanner.Scan();

        IReadOnlyList<LibraryGame> libraryGames =
            await libraryBuilder.BuildAsync(
                files,
                cancellationToken);

        IReadOnlyList<SteamShortcut> steamShortcuts =
            steamGameBuilder.Build(libraryGames);

        await steamIntegrationService.ExportAsync(
            steamShortcuts,
            cancellationToken);

        return steamShortcuts.Count;
    }
}