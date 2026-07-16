using System.Text.Json;
using EmulationManager.Models;

namespace EmulationManager.Steam;

public sealed class SteamRomManagerExportService
    : ISteamIntegrationService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            WriteIndented = true
        };

    private readonly string exportDirectory;

    public SteamRomManagerExportService()
    {
        exportDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "EmulationManager",
            "SteamExport");
    }

    public string Name =>
        "Steam ROM Manager Export";

    public bool IsAvailable => true;

    public Task ExportAsync(
        IReadOnlyCollection<SteamGameEntry> games,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(exportDirectory);

        var manifest = new SteamExportManifest
        {
            Games = games
                .OrderBy(game => game.ConsoleName)
                .ThenBy(game => game.Title)
                .ToList()
        };

        string manifestPath =
            Path.Combine(
                exportDirectory,
                "steam-games.json");

        string json =
            JsonSerializer.Serialize(
                manifest,
                JsonOptions);

        File.WriteAllText(
            manifestPath,
            json);

        WriteParserInstructions();

        return Task.CompletedTask;
    }

    private void WriteParserInstructions()
    {
        string instructionsPath =
            Path.Combine(
                exportDirectory,
                "README-Steam-Import.txt");

        string text =
            """
            Emulation Manager Steam Export

            This folder was generated for Steam integration.

            steam-games.json contains:
            - Game title
            - Console
            - ROM path
            - Emulation Manager executable
            - Launch arguments

            The next project stage will generate and manage the
            Steam ROM Manager parser automatically.

            Do not delete this folder while Steam synchronization
            is being configured.
            """;

        File.WriteAllText(
            instructionsPath,
            text);
    }
}
