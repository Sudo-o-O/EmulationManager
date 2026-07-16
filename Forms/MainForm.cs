using System.Diagnostics;
using EmulationManager.Configuration;
using EmulationManager.Models;
using EmulationManager.Services;
using EmulationManager.Utilities;

namespace EmulationManager.Forms;

public sealed class MainForm : Form
{
    private readonly AppPaths paths;
    private readonly LibraryScanner libraryScanner;
    private readonly EmulatorService emulatorService;
    private readonly SettingsService settingsService;

    private readonly ListView gameList = new();
    private readonly Label romStatusLabel = new();
    private readonly Label stableStatusLabel = new();
    private readonly Label nightlyStatusLabel = new();
    private readonly Label summaryLabel = new();

    public MainForm(
        AppPaths paths,
        LibraryScanner libraryScanner,
        EmulatorService emulatorService,
        SettingsService settingsService)
    {
        this.paths = paths;
        this.libraryScanner = libraryScanner;
        this.emulatorService = emulatorService;
        this.settingsService = settingsService;

        Text = "Emulation Manager TEST";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 600);
        Size = new Size(1100, 700);

        BuildInterface();

        Shown += (_, _) =>
            ScanEverything();
    }

    private void BuildInterface()
    {
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 160,
            Padding = new Padding(16)
        };

        var titleLabel = new Label
        {
            Text = "Emulation Manager",
            AutoSize = true,
            Font = new Font(
                "Segoe UI",
                20,
                FontStyle.Bold),
            Location = new Point(16, 12)
        };

        romStatusLabel.AutoSize = true;
        romStatusLabel.Location = new Point(18, 58);

        stableStatusLabel.AutoSize = true;
        stableStatusLabel.Location = new Point(18, 82);

        nightlyStatusLabel.AutoSize = true;
        nightlyStatusLabel.Location = new Point(18, 106);

        var scanButton = new Button
        {
            Text = "Scan Library",
            Width = 140,
            Height = 34,
            Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right
        };

        var openFolderButton = new Button
        {
            Text = "Open ROM Folder",
            Width = 140,
            Height = 34,
            Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right
        };

        var launchButton = new Button
        {
            Text = "Launch Selected",
            Width = 140,
            Height = 34,
            Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right
        };

        var settingsButton = new Button
        {
            Text = "Settings",
            Width = 120,
            Height = 34,
            Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right
        };

        scanButton.Click += (_, _) =>
            ScanEverything();

        openFolderButton.Click += (_, _) =>
            OpenFolder(paths.RomRoot);

        launchButton.Click += (_, _) =>
            LaunchSelectedGame();

        settingsButton.Click += (_, _) =>
            OpenSettings();

        topPanel.Resize += (_, _) =>
        {
            int right =
                topPanel.ClientSize.Width - 20;

            settingsButton.Location =
                new Point(
                    right - settingsButton.Width,
                    16);

            scanButton.Location =
                new Point(
                    right - scanButton.Width,
                    58);

            openFolderButton.Location =
                new Point(
                    right - openFolderButton.Width,
                    98);

            launchButton.Location =
                new Point(
                    right - launchButton.Width - 155,
                    98);
        };

        topPanel.Controls.Add(titleLabel);
        topPanel.Controls.Add(romStatusLabel);
        topPanel.Controls.Add(stableStatusLabel);
        topPanel.Controls.Add(nightlyStatusLabel);
        topPanel.Controls.Add(scanButton);
        topPanel.Controls.Add(openFolderButton);
        topPanel.Controls.Add(launchButton);
        topPanel.Controls.Add(settingsButton);

        summaryLabel.Dock = DockStyle.Top;
        summaryLabel.Height = 38;
        summaryLabel.Padding =
            new Padding(16, 8, 0, 0);

        summaryLabel.Font =
            new Font(
                "Segoe UI",
                10,
                FontStyle.Bold);

        gameList.Dock = DockStyle.Fill;
        gameList.View = View.Details;
        gameList.FullRowSelect = true;
        gameList.GridLines = true;
        gameList.HideSelection = false;
        gameList.MultiSelect = false;

        gameList.Columns.Add("Game", 230);
        gameList.Columns.Add("Console", 150);
        gameList.Columns.Add("Type", 110);
        gameList.Columns.Add("Filename", 360);
        gameList.Columns.Add("Size", 100);
        gameList.Columns.Add("Full Path", 600);

        gameList.DoubleClick += (_, _) =>
            LaunchSelectedGame();

        Controls.Add(gameList);
        Controls.Add(summaryLabel);
        Controls.Add(topPanel);
    }

    private void OpenSettings()
    {
        AppSettings currentSettings =
            settingsService.Load();

        using var form =
            new SettingsForm(
                settingsService,
                currentSettings);

        if (form.ShowDialog(this) !=
            DialogResult.OK)
        {
            return;
        }

        if (form.SavedSettings is null)
        {
            return;
        }

        paths.Apply(form.SavedSettings);
        ScanEverything();
    }

    private void ScanEverything()
    {
        CheckPaths();
        ScanLibrary();
    }

    private void CheckPaths()
    {
        romStatusLabel.Text =
            Directory.Exists(paths.RomRoot)
                ? $"ROM library: Found - {paths.RomRoot}"
                : $"ROM library: Missing - {paths.RomRoot}";

        stableStatusLabel.Text =
            emulatorService.EdenStable.IsInstalled
                ? $"Eden Stable: Found - " +
                  emulatorService.EdenStable.ExecutablePath
                : $"Eden Stable: Missing - " +
                  emulatorService.EdenStable.ExecutablePath;

        nightlyStatusLabel.Text =
            emulatorService.EdenNightly.IsInstalled
                ? $"Eden Nightly: Found - " +
                  emulatorService.EdenNightly.ExecutablePath
                : $"Eden Nightly: Missing - " +
                  emulatorService.EdenNightly.ExecutablePath;
    }

    private void ScanLibrary()
    {
        gameList.BeginUpdate();
        gameList.Items.Clear();

        try
        {
            IReadOnlyList<GameFile> files =
                libraryScanner.Scan();

            foreach (GameFile gameFile in files)
            {
                var item =
                    new ListViewItem(
                        gameFile.GameName);

                item.SubItems.Add(
                    gameFile.ConsoleName);

                item.SubItems.Add(
                    gameFile.ContentType);

                item.SubItems.Add(
                    gameFile.FileName);

                item.SubItems.Add(
                    FileSizeFormatter.Format(
                        gameFile.SizeBytes));

                item.SubItems.Add(
                    gameFile.FullPath);

                item.Tag = gameFile;
                gameList.Items.Add(item);
            }

            int gameCount =
                files.Select(file =>
                        $"{file.ConsoleName}|{file.GameName}")
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .Count();

            summaryLabel.Text =
                $"Found {gameCount} game folder(s) and " +
                $"{files.Count} supported game file(s).";
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Scan Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            gameList.EndUpdate();
        }
    }

    private void LaunchSelectedGame()
    {
        if (gameList.SelectedItems.Count == 0)
        {
            MessageBox.Show(
                "Select a base game first.",
                "Nothing Selected",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (gameList.SelectedItems[0].Tag
            is not GameFile gameFile)
        {
            return;
        }

        if (gameFile.ContentType is
            "Update" or "DLC")
        {
            MessageBox.Show(
                "Select the base game, not an update or DLC.",
                "Select Base Game",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        if (!gameFile.ConsoleName.Equals(
                "Switch",
                StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                $"{gameFile.ConsoleName} support " +
                "will be added later.",
                "Emulator Not Configured",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        AppSettings currentSettings =
            settingsService.Load();

        using var dialog =
            new LaunchDialog(
                new LaunchArguments
                {
                    RomPath = gameFile.FullPath
                },
                emulatorService,
                settingsService,
                currentSettings);

        dialog.ShowDialog(this);
    }

    private static void OpenFolder(
        string folder)
    {
        if (!Directory.Exists(folder))
        {
            MessageBox.Show(
                $"Folder not found:\n\n{folder}",
                "Folder Missing",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return;
        }

        Process.Start(
            new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
    }
}

