using EmulationManager.Configuration;
using EmulationManager.Forms;
using EmulationManager.LaunchStrategies;
using EmulationManager.Models;
using EmulationManager.Services;



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
