using System.Text.Json;
using EmulationManager.Configuration;

namespace EmulationManager.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

    public string SettingsDirectory { get; }

    public string SettingsFilePath { get; }

    public SettingsService()
    {
        SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "EmulationManager");

        SettingsFilePath = Path.Combine(
            SettingsDirectory,
            "settings.json");
    }

    public AppSettings Load()
    {
        Directory.CreateDirectory(SettingsDirectory);

        if (!File.Exists(SettingsFilePath))
        {
            AppSettings defaults = new();
            Save(defaults);
            return defaults;
        }

        try
        {
            string json = File.ReadAllText(SettingsFilePath);

            AppSettings? settings =
                JsonSerializer.Deserialize<AppSettings>(
                    json,
                    JsonOptions);

            if (settings is null)
            {
                throw new InvalidDataException(
                    "The settings file was empty.");
            }

            ApplyMissingDefaults(settings);
            return settings;
        }
        catch
        {
            string backupPath =
                SettingsFilePath +
                ".invalid-" +
                DateTime.Now.ToString("yyyyMMdd-HHmmss");

            File.Copy(
                SettingsFilePath,
                backupPath,
                overwrite: true);

            AppSettings defaults = new();
            Save(defaults);
            return defaults;
        }
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(SettingsDirectory);

        string json =
            JsonSerializer.Serialize(settings, JsonOptions);

        File.WriteAllText(SettingsFilePath, json);
    }

    private static void ApplyMissingDefaults(
        AppSettings settings)
    {
        AppSettings defaults = new();

        if (string.IsNullOrWhiteSpace(settings.RomRoot))
        {
            settings.RomRoot = defaults.RomRoot;
        }

        if (string.IsNullOrWhiteSpace(
                settings.EdenStablePath))
        {
            settings.EdenStablePath =
                defaults.EdenStablePath;
        }

        if (string.IsNullOrWhiteSpace(
                settings.EdenNightlyPath))
        {
            settings.EdenNightlyPath =
                defaults.EdenNightlyPath;
        }

        settings.EdenStableLauncher ??= new LauncherSettings();
        settings.EdenNightlyLauncher ??= new LauncherSettings();

        settings.EdenStableLauncher.GameOverrides ??=
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

        settings.EdenNightlyLauncher.GameOverrides ??=
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
    }
}
