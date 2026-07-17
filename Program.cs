using EmulationManager.Configuration;
using EmulationManager.Forms;
using EmulationManager.LaunchStrategies;
using EmulationManager.Models;
using EmulationManager.Services;
using EmulationManager.Metadata;
using EmulationManager.Switch;
using EmulationManager.Steam;



namespace EmulationManager;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        SettingsService settingsService = new();
        AppSettings settings = settingsService.Load();
        AppPaths paths = AppPaths.FromSettings(settings);

        WindowsShortcutService shortcutService = new();

        ILaunchStrategy[] strategies =
        [
            new DirectLaunchStrategy(),
            new DetachedLaunchStrategy(shortcutService)
        ];

        LaunchStrategyFactory strategyFactory =
            new(strategies);

        EmulatorService emulatorService =
            new(paths, strategyFactory);

        LibraryScanner libraryScanner = new(paths);


        SwitchPackageValidator switchPackageValidator =
            new();

        SwitchPackageReader switchPackageReader =
            new(switchPackageValidator);

        SwitchKeyLoader switchKeyLoader =
            new();

        GameMetadataService metadataService =
            new(
            [
                new SwitchMetadataProvider(
                    switchPackageReader,
                    switchKeyLoader,
                    settings.Switch.ProdKeysPath),

                new FilenameSwitchMetadataProvider()
            ]);

        GameIdService gameIdService =
            new(metadataService);

        LibraryBuilder libraryBuilder =
            new(gameIdService);
        
        GameLibraryService gameLibraryService =
            new(
                libraryScanner,
                libraryBuilder);

        SteamGameBuilder steamGameBuilder =
            new();

        SteamRomManagerExportService steamIntegrationService =
            new();

        SteamExportService steamExportService =
            new(
                libraryScanner,
                libraryBuilder,
                steamGameBuilder,
                steamIntegrationService);

        if (LaunchArguments.TryParse(
                args,
                out LaunchArguments? launchArguments))
        {
            LaunchArguments resolvedArguments =
                launchArguments!;

            if (!string.IsNullOrWhiteSpace(
                    launchArguments!.GameId))
            {
                LibraryGame? game =
                    gameLibraryService
                        .FindByIdAsync(
                            launchArguments.GameId)
                        .GetAwaiter()
                        .GetResult();

                if (game is null)
                {
                    MessageBox.Show(
                        $"The game could not be found:\n\n" +
                        launchArguments.GameId,
                        "Game Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                resolvedArguments =
                    new LaunchArguments
                    {
                        RomPath =
                            game.BaseGame.FullPath,

                        GameId =
                            game.Id
                    };
            }

            Application.Run(
                new LaunchDialog(
                    resolvedArguments,
                    emulatorService,
                    settingsService,
                    settings));

            return;
        }

        Application.Run(
            new MainForm(
                paths,
                libraryScanner,
                emulatorService,
                settingsService,
                steamExportService));
    }
}
