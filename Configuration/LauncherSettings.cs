namespace EmulationManager.Configuration;

public sealed class LauncherSettings
{
    public string DefaultLaunchMethod { get; set; } = "Auto";

    public Dictionary<string, string> GameOverrides { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}
