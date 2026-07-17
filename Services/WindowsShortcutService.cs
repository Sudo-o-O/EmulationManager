using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EmulationManager.Services;

public sealed class WindowsShortcutService
{
    private readonly string shortcutDirectory;

    public WindowsShortcutService()
    {
        shortcutDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "EmulationManager",
            "Shortcuts");

        Directory.CreateDirectory(shortcutDirectory);
    }

    public async Task<Process> LaunchAsync(
        string launcherName,
        string emulatorPath,
        string romPath)
    {
        string shortcutPath = CreateShortcut(
            launcherName,
            emulatorPath,
            romPath);

        HashSet<int> existingProcessIds =
            GetMatchingProcessIds(emulatorPath);

        Process.Start(
            new ProcessStartInfo
            {
                FileName = "explorer.exe",
                ArgumentList =
                {
                    shortcutPath
                },
                UseShellExecute = true
            });

        DateTime deadline =
            DateTime.UtcNow.AddSeconds(15);

        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(250);

            Process? newProcess =
                FindNewProcess(
                    emulatorPath,
                    existingProcessIds);

            if (newProcess is not null)
            {
                return newProcess;
            }
        }

        throw new InvalidOperationException(
            "Windows launched the shortcut, but the emulator process " +
            "could not be detected within 15 seconds.");
    }

    private string CreateShortcut(
        string launcherName,
        string emulatorPath,
        string romPath)
    {
        string safeName =
            string.Concat(
                launcherName.Select(character =>
                    Path.GetInvalidFileNameChars().Contains(character)
                        ? '_'
                        : character));

        string shortcutPath =
            Path.Combine(
                shortcutDirectory,
                safeName + ".lnk");

        Type shellType =
            Type.GetTypeFromProgID("WScript.Shell")
            ?? throw new InvalidOperationException(
                "Windows Script Host could not be found.");

        dynamic? shell = null;
        dynamic? shortcut = null;

        try
        {
            shell = Activator.CreateInstance(shellType)
                ?? throw new InvalidOperationException(
                    "Windows Script Host could not be started.");

            shortcut =
                shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = emulatorPath;
            shortcut.Arguments = QuoteArgument(romPath);

            shortcut.WorkingDirectory =
                Path.GetDirectoryName(emulatorPath);

            shortcut.Description =
                $"Launch {launcherName} through Emulation Manager";

            shortcut.Save();
        }
        finally
        {
            if (shortcut is not null &&
                Marshal.IsComObject(shortcut))
            {
                Marshal.FinalReleaseComObject(shortcut);
            }

            if (shell is not null &&
                Marshal.IsComObject(shell))
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        return shortcutPath;
    }

    private static HashSet<int> GetMatchingProcessIds(
        string executablePath)
    {
        string processName =
            Path.GetFileNameWithoutExtension(executablePath);

        var ids = new HashSet<int>();

        foreach (Process process in
                 Process.GetProcessesByName(processName))
        {
            try
            {
                if (PathsEqual(
                        process.MainModule?.FileName,
                        executablePath))
                {
                    ids.Add(process.Id);
                }
            }
            catch
            {
                // Ignore inaccessible processes.
            }
            finally
            {
                process.Dispose();
            }
        }

        return ids;
    }

    private static Process? FindNewProcess(
        string executablePath,
        HashSet<int> existingIds)
    {
        string processName =
            Path.GetFileNameWithoutExtension(executablePath);

        foreach (Process process in
                 Process.GetProcessesByName(processName))
        {
            try
            {
                if (!existingIds.Contains(process.Id) &&
                    PathsEqual(
                        process.MainModule?.FileName,
                        executablePath))
                {
                    return process;
                }
            }
            catch
            {
                process.Dispose();
            }
        }

        return null;
    }

    private static bool PathsEqual(
        string? first,
        string second)
    {
        if (string.IsNullOrWhiteSpace(first))
        {
            return false;
        }

        return string.Equals(
            Path.GetFullPath(first),
            Path.GetFullPath(second),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string QuoteArgument(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }
}
