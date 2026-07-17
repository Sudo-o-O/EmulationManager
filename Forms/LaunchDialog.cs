using System.Diagnostics;
using EmulationManager.Configuration;
using EmulationManager.Models;
using EmulationManager.Services;

namespace EmulationManager.Forms;

public sealed class LaunchDialog : Form
{
    private readonly LaunchArguments launchArguments;
    private readonly string romPath;
    private readonly EmulatorService emulatorService;
    private readonly SettingsService settingsService;
    private readonly AppSettings settings;

    private readonly ComboBox launcherComboBox = new();
    private readonly ComboBox methodComboBox = new();
    private readonly CheckBox rememberCheckBox = new();
    private readonly Button launchButton = new();
    private readonly Label statusLabel = new();

    public LaunchDialog(
        LaunchArguments launchArguments,
        EmulatorService emulatorService,
        SettingsService settingsService,
        AppSettings settings)
    {
        string resolvedRomPath =
            launchArguments.RomPath
            ?? throw new ArgumentException(
                "A ROM path is required to open the launch dialog.",
                nameof(launchArguments));

        if (string.IsNullOrWhiteSpace(resolvedRomPath))
        {
            throw new ArgumentException(
                "A ROM path is required to open the launch dialog.",
                nameof(launchArguments));
        }

        romPath = resolvedRomPath;

        this.launchArguments = launchArguments;
        this.emulatorService = emulatorService;
        this.settingsService = settingsService;
        this.settings = settings;

        Text = "Launch Game";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(500, 330);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        BuildInterface();
        LoadSavedChoice();
    }

    private void BuildInterface()
    {
        var titleLabel = new Label
        {
            Text = Path.GetFileNameWithoutExtension(
                romPath),

            AutoEllipsis = true,
            Font = new Font(
                "Segoe UI",
                15,
                FontStyle.Bold),

            Location = new Point(24, 18),
            Size = new Size(450, 40)
        };

        var launcherLabel = new Label
        {
            Text = "Launcher",
            AutoSize = true,
            Location = new Point(25, 72)
        };

        launcherComboBox.DropDownStyle =
            ComboBoxStyle.DropDownList;

        launcherComboBox.Location =
            new Point(25, 94);

        launcherComboBox.Size =
            new Size(440, 28);

        launcherComboBox.Items.Add("Eden Stable");
        launcherComboBox.Items.Add("Eden Nightly");
        launcherComboBox.SelectedIndex = 0;

        launcherComboBox.SelectedIndexChanged +=
            (_, _) => LoadSavedChoice();

        var methodLabel = new Label
        {
            Text = "Launch method",
            AutoSize = true,
            Location = new Point(25, 138)
        };

        methodComboBox.DropDownStyle =
            ComboBoxStyle.DropDownList;

        methodComboBox.Location =
            new Point(25, 160);

        methodComboBox.Size =
            new Size(440, 28);

        methodComboBox.Items.Add("Use launcher/game default");
        methodComboBox.Items.Add("Auto");
        methodComboBox.Items.Add("Direct");
        methodComboBox.Items.Add("Detached");
        methodComboBox.SelectedIndex = 0;

        rememberCheckBox.Text =
            "Remember this method for this game and launcher";

        rememberCheckBox.AutoSize = true;
        rememberCheckBox.Location =
            new Point(25, 205);

        statusLabel.Text =
            "Ready.";

        statusLabel.AutoEllipsis = true;
        statusLabel.Location =
            new Point(25, 235);

        statusLabel.Size =
            new Size(300, 50);

        launchButton.Text = "Launch";
        launchButton.Size = new Size(105, 38);
        launchButton.Location = new Point(246, 270);

        var cancelButton = new Button
        {
            Text = "Cancel",
            Size = new Size(105, 38),
            Location = new Point(360, 270),
            DialogResult = DialogResult.Cancel
        };

        launchButton.Click += async (_, _) =>
            await LaunchAsync();

        AcceptButton = launchButton;
        CancelButton = cancelButton;

        Controls.Add(titleLabel);
        Controls.Add(launcherLabel);
        Controls.Add(launcherComboBox);
        Controls.Add(methodLabel);
        Controls.Add(methodComboBox);
        Controls.Add(rememberCheckBox);
        Controls.Add(statusLabel);
        Controls.Add(launchButton);
        Controls.Add(cancelButton);
    }

    private async Task LaunchAsync()
    {
        try
        {
            launchButton.Enabled = false;

            EmulatorBuild emulator =
                GetSelectedEmulator();

            LaunchMethod method =
                GetSelectedMethod(emulator);

            if (method == LaunchMethod.Ask)
            {
                method = AskForMethod();

                if (method == LaunchMethod.Ask)
                {
                    launchButton.Enabled = true;
                    return;
                }
            }

            if (rememberCheckBox.Checked)
            {
                SaveGameOverride(emulator, method);
            }

            statusLabel.Text =
                $"Launching {emulator.Name} using {method}...";

            Process process =
                await emulatorService.LaunchAsync(
                    emulator,
                    romPath,
                    method);

            Hide();

            try
            {
                await process.WaitForExitAsync();
            }
            catch
            {
                // Some shell-launched processes may become inaccessible.
            }

            Close();
        }
        catch (Exception exception)
        {
            Show();
            launchButton.Enabled = true;
            statusLabel.Text = "Launch failed.";

            MessageBox.Show(
                exception.Message,
                "Launch Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private EmulatorBuild GetSelectedEmulator()
    {
        return launcherComboBox.SelectedIndex == 1
            ? emulatorService.EdenNightly
            : emulatorService.EdenStable;
    }

    private LaunchMethod GetSelectedMethod(
        EmulatorBuild emulator)
    {
        if (methodComboBox.SelectedIndex > 0)
        {
            return methodComboBox.SelectedIndex switch
            {
                1 => LaunchMethod.Auto,
                2 => LaunchMethod.Direct,
                3 => LaunchMethod.Detached,
                _ => LaunchMethod.Auto
            };
        }

        LauncherSettings launcherSettings =
            GetLauncherSettings(emulator);

        string gameKey =
            NormalizeGameKey(
                romPath);

        if (launcherSettings.GameOverrides.TryGetValue(
                gameKey,
                out string? overrideValue) &&
            Enum.TryParse(
                overrideValue,
                ignoreCase: true,
                out LaunchMethod overrideMethod))
        {
            return overrideMethod;
        }

        if (Enum.TryParse(
                launcherSettings.DefaultLaunchMethod,
                ignoreCase: true,
                out LaunchMethod defaultMethod))
        {
            return defaultMethod;
        }

        return LaunchMethod.Auto;
    }

    private void LoadSavedChoice()
    {
        if (launcherComboBox.SelectedIndex < 0)
        {
            return;
        }

        EmulatorBuild emulator =
            GetSelectedEmulator();

        LauncherSettings launcherSettings =
            GetLauncherSettings(emulator);

        string gameKey =
            NormalizeGameKey(
                romPath);

        if (launcherSettings.GameOverrides.TryGetValue(
                gameKey,
                out string? savedMethod))
        {
            statusLabel.Text =
                $"Saved game method: {savedMethod}";
        }
        else
        {
            statusLabel.Text =
                $"Launcher default: " +
                launcherSettings.DefaultLaunchMethod;
        }
    }

    private void SaveGameOverride(
        EmulatorBuild emulator,
        LaunchMethod method)
    {
        LauncherSettings launcherSettings =
            GetLauncherSettings(emulator);

        launcherSettings.GameOverrides[
            NormalizeGameKey(romPath)] =
            method.ToString();

        settingsService.Save(settings);
    }

    private LauncherSettings GetLauncherSettings(
        EmulatorBuild emulator)
    {
        return emulator.Name.Equals(
                "Eden Nightly",
                StringComparison.OrdinalIgnoreCase)
            ? settings.EdenNightlyLauncher
            : settings.EdenStableLauncher;
    }

    private static LaunchMethod AskForMethod()
    {
        DialogResult result =
            MessageBox.Show(
                "Use Direct launch?\n\n" +
                "Select No to use the Detached launch method.",
                "Choose Launch Method",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

        return result switch
        {
            DialogResult.Yes => LaunchMethod.Direct,
            DialogResult.No => LaunchMethod.Detached,
            _ => LaunchMethod.Ask
        };
    }

    private static string NormalizeGameKey(
        string romPath)
    {
        return Path.GetFullPath(romPath)
            .Trim()
            .ToUpperInvariant();
    }
}

