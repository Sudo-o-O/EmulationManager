namespace EmulationManager.Configuration;

public sealed class AppPaths
{
    public string RomRoot { get; set; } = string.Empty;

    public string EdenStablePath { get; set; } = string.Empty;

    public string EdenNightlyPath { get; set; } = string.Empty;

    public static AppPaths FromSettings(AppSettings settings)
    {
        return new AppPaths
        {
            RomRoot = settings.RomRoot,
            EdenStablePath = settings.EdenStablePath,
            EdenNightlyPath = settings.EdenNightlyPath
        };
    }

    public void Apply(AppSettings settings)
    {
        RomRoot = settings.RomRoot;
        EdenStablePath = settings.EdenStablePath;
        EdenNightlyPath = settings.EdenNightlyPath;
    }
}
