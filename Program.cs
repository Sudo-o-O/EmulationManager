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

        SteamGameBuilder steamGameBuilder =
            new(gameIdService);

        SteamRomManagerExportService steamIntegrationService =
            new();

        SteamExportService steamExportService =
            new(
                libraryScanner,
                steamGameBuilder,
                steamIntegrationService);

        if (LaunchArguments.TryParse(
                args,
                out LaunchArguments? launchArguments))
        {
            Application.Run(
                new LaunchDialog(
                    launchArguments!,
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
                settingsService));
    }
}
