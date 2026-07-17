namespace EmulationManager.Configuration;

public sealed class AppSettings
{
    public string RomRoot { get; set; } =
        @"D:\Games\ROMs";

    public string EdenStablePath { get; set; } =
        @"C:\Emulation\Emulators\Eden\eden.exe";

    public string EdenNightlyPath { get; set; } =
        @"C:\Emulation\Emulators\Eden Nightly\eden.exe";

    public LauncherSettings EdenStableLauncher { get; set; } =
        new();

    public LauncherSettings EdenNightlyLauncher { get; set; } =
        new();

    public SwitchSettings Switch { get; set; } = new();
}
