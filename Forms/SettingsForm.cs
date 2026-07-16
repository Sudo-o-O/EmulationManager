using System.Diagnostics;
using EmulationManager.Configuration;
using EmulationManager.Models;
using EmulationManager.Services;

namespace EmulationManager.Forms;

public sealed class SettingsForm : Form
{
    private readonly SettingsService settingsService;
    private readonly AppSettings currentSettings;

    private readonly TextBox romRootTextBox = new();
    private readonly TextBox stableTextBox = new();
    private readonly TextBox nightlyTextBox = new();

    private readonly ComboBox stableMethodComboBox = new();
    private readonly ComboBox nightlyMethodComboBox = new();

    public AppSettings? SavedSettings { get; private set; }

    public SettingsForm(
        SettingsService settingsService,
        AppSettings currentSettings)
    {
        this.settingsService = settingsService;
        this.currentSettings = currentSettings;

        Text = "Emulation Manager Settings";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(780, 540);
        MinimumSize = new Size(780, 540);
        MaximumSize = new Size(780, 540);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        BuildInterface();
    }

    private void BuildInterface()
    {
        var titleLabel = new Label
        {
            Text = "Library and launcher settings",
            AutoSize = true,
            Font = new Font(
                "Segoe UI",
                16,
                FontStyle.Bold),
            Location = new Point(22, 18)
        };

        var helpLabel = new Label
        {
            Text =
                "Choose your ROM folder, Eden executables, " +
                "and the default launch method for each launcher.",
            AutoSize = true,
            Location = new Point(24, 56)
        };

        var romLabel = CreateLabel(
            "ROM library folder",
            24,
            94);

        romRootTextBox.Text = currentSettings.RomRoot;
        romRootTextBox.Location = new Point(24, 116);
        romRootTextBox.Size = new Size(610, 27);

        var romBrowseButton = CreateBrowseButton(
            648,
            114,
            (_, _) => BrowseForRomFolder());

        var stableLabel = CreateLabel(
            "Eden Stable executable",
            24,
            160);

        stableTextBox.Text =
            currentSettings.EdenStablePath;

        stableTextBox.Location =
            new Point(24, 182);

        stableTextBox.Size =
            new Size(610, 27);

        var stableBrowseButton = CreateBrowseButton(
            648,
            180,
            (_, _) => BrowseForExecutable(stableTextBox));

        var stableMethodLabel = CreateLabel(
            "Eden Stable default launch method",
            24,
            222);

        ConfigureMethodComboBox(
            stableMethodComboBox,
            currentSettings
                .EdenStableLauncher
                .DefaultLaunchMethod,
            24,
            244);

        var nightlyLabel = CreateLabel(
            "Eden Nightly executable",
            24,
            292);

        nightlyTextBox.Text =
            currentSettings.EdenNightlyPath;

        nightlyTextBox.Location =
            new Point(24, 314);

        nightlyTextBox.Size =
            new Size(610, 27);

        var nightlyBrowseButton = CreateBrowseButton(
            648,
            312,
            (_, _) => BrowseForExecutable(nightlyTextBox));

        var nightlyMethodLabel = CreateLabel(
            "Eden Nightly default launch method",
            24,
            354);

        ConfigureMethodComboBox(
            nightlyMethodComboBox,
            currentSettings
                .EdenNightlyLauncher
                .DefaultLaunchMethod,
            24,
            376);

        var explanationLabel = new Label
        {
            Text =
                "Auto: uses Detached for Eden on Windows.  " +
                "Direct: starts Eden directly.  " +
                "Detached: launches through Explorer and a generated shortcut.  " +
                "Ask: prompts each time.",
            Location = new Point(24, 420),
            Size = new Size(710, 42)
        };

        var openSettingsButton = new Button
        {
            Text = "Open Settings Folder",
            Location = new Point(24, 476),
            Size = new Size(165, 36)
        };

        openSettingsButton.Click +=
            (_, _) => OpenSettingsFolder();

        var saveButton = new Button
        {
            Text = "Save",
            Location = new Point(526, 476),
            Size = new Size(105, 36)
        };

        saveButton.Click +=
            (_, _) => SaveSettings();

        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(648, 476),
            Size = new Size(105, 36),
            DialogResult = DialogResult.Cancel
        };

        AcceptButton = saveButton;
        CancelButton = cancelButton;

        Controls.Add(titleLabel);
        Controls.Add(helpLabel);

        Controls.Add(romLabel);
        Controls.Add(romRootTextBox);
        Controls.Add(romBrowseButton);

        Controls.Add(stableLabel);
        Controls.Add(stableTextBox);
        Controls.Add(stableBrowseButton);
        Controls.Add(stableMethodLabel);
        Controls.Add(stableMethodComboBox);

        Controls.Add(nightlyLabel);
        Controls.Add(nightlyTextBox);
        Controls.Add(nightlyBrowseButton);
        Controls.Add(nightlyMethodLabel);
        Controls.Add(nightlyMethodComboBox);

        Controls.Add(explanationLabel);
        Controls.Add(openSettingsButton);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
    }

    private static Label CreateLabel(
        string text,
        int left,
        int top)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Location = new Point(left, top)
        };
    }

    private static Button CreateBrowseButton(
        int left,
        int top,
        EventHandler clickHandler)
    {
        var button = new Button
        {
            Text = "Browse...",
            Location = new Point(left, top),
            Size = new Size(105, 31)
        };

        button.Click += clickHandler;
        return button;
    }

    private static void ConfigureMethodComboBox(
        ComboBox comboBox,
        string savedValue,
        int left,
        int top)
    {
        comboBox.DropDownStyle =
            ComboBoxStyle.DropDownList;

        comboBox.Location =
            new Point(left, top);

        comboBox.Size =
            new Size(260, 28);

        comboBox.Items.AddRange(
        [
            LaunchMethod.Auto.ToString(),
            LaunchMethod.Direct.ToString(),
            LaunchMethod.Detached.ToString(),
            LaunchMethod.Ask.ToString()
        ]);

        int savedIndex =
            comboBox.Items.IndexOf(savedValue);

        comboBox.SelectedIndex =
            savedIndex >= 0 ? savedIndex : 0;
    }

    private void BrowseForRomFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description =
                "Select the folder containing console folders " +
                "such as Switch, PS2, and GameCube.",

            ShowNewFolderButton = true,

            InitialDirectory =
                Directory.Exists(romRootTextBox.Text)
                    ? romRootTextBox.Text
                    : string.Empty
        };

        if (dialog.ShowDialog(this) ==
            DialogResult.OK)
        {
            romRootTextBox.Text =
                dialog.SelectedPath;
        }
    }

    private void BrowseForExecutable(
        TextBox destinationTextBox)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select the emulator executable",

            Filter =
                "Executable files (*.exe)|*.exe|" +
                "All files (*.*)|*.*",

            CheckFileExists = true,
            Multiselect = false
        };

        string currentPath =
            destinationTextBox.Text;

        if (File.Exists(currentPath))
        {
            dialog.InitialDirectory =
                Path.GetDirectoryName(currentPath);

            dialog.FileName =
                Path.GetFileName(currentPath);
        }

        if (dialog.ShowDialog(this) ==
            DialogResult.OK)
        {
            destinationTextBox.Text =
                dialog.FileName;
        }
    }

    private void SaveSettings()
    {
        string romRoot =
            romRootTextBox.Text.Trim();

        string stablePath =
            stableTextBox.Text.Trim();

        string nightlyPath =
            nightlyTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(romRoot))
        {
            MessageBox.Show(
                "Choose a ROM library folder.",
                "ROM Folder Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            romRootTextBox.Focus();
            return;
        }

        if (!ValidatePath(
                romRoot,
                isDirectory: true,
                "ROM library folder"))
        {
            return;
        }

        if (!ValidateOptionalExecutable(
                stablePath,
                "Eden Stable"))
        {
            return;
        }

        if (!ValidateOptionalExecutable(
                nightlyPath,
                "Eden Nightly"))
        {
            return;
        }

        SavedSettings = new AppSettings
        {
            RomRoot = romRoot,
            EdenStablePath = stablePath,
            EdenNightlyPath = nightlyPath,

            EdenStableLauncher = new LauncherSettings
            {
                DefaultLaunchMethod =
                    stableMethodComboBox
                        .SelectedItem?
                        .ToString()
                    ?? LaunchMethod.Auto.ToString(),

                GameOverrides =
                    currentSettings
                        .EdenStableLauncher
                        .GameOverrides
            },

            EdenNightlyLauncher = new LauncherSettings
            {
                DefaultLaunchMethod =
                    nightlyMethodComboBox
                        .SelectedItem?
                        .ToString()
                    ?? LaunchMethod.Auto.ToString(),

                GameOverrides =
                    currentSettings
                        .EdenNightlyLauncher
                        .GameOverrides
            }
        };

        settingsService.Save(SavedSettings);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidatePath(
        string path,
        bool isDirectory,
        string displayName)
    {
        bool exists =
            isDirectory
                ? Directory.Exists(path)
                : File.Exists(path);

        if (exists)
        {
            return true;
        }

        DialogResult result =
            MessageBox.Show(
                $"{displayName} was not found:\n\n" +
                path +
                "\n\nSave it anyway?",
                $"{displayName} Not Found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

        return result == DialogResult.Yes;
    }

    private bool ValidateOptionalExecutable(
        string path,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return true;
        }

        return ValidatePath(
            path,
            isDirectory: false,
            displayName);
    }

    private void OpenSettingsFolder()
    {
        Directory.CreateDirectory(
            settingsService.SettingsDirectory);

        Process.Start(
            new ProcessStartInfo
            {
                FileName =
                    settingsService.SettingsDirectory,

                UseShellExecute = true
            });
    }
}
