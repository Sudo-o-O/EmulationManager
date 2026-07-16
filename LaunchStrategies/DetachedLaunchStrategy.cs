using System.Diagnostics;
using EmulationManager.Models;
using EmulationManager.Services;

namespace EmulationManager.LaunchStrategies;

public sealed class DetachedLaunchStrategy : ILaunchStrategy
{
    private readonly WindowsShortcutService shortcutService;

    public DetachedLaunchStrategy(
        WindowsShortcutService shortcutService)
    {
        this.shortcutService = shortcutService;
    }

    public LaunchMethod Method => LaunchMethod.Detached;

    public Task<Process> LaunchAsync(
        LaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        return shortcutService.LaunchAsync(
            request.DisplayName,
            request.Emulator.ExecutablePath,
            request.RomPath);
    }

    private static void Validate(LaunchRequest request)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "The Windows detached launcher is only available on Windows.");
        }

        if (!request.Emulator.IsInstalled)
        {
            throw new FileNotFoundException(
                $"{request.Emulator.Name} was not found.",
                request.Emulator.ExecutablePath);
        }

        if (!File.Exists(request.RomPath))
        {
            throw new FileNotFoundException(
                "The selected game file was not found.",
                request.RomPath);
        }
    }
}
