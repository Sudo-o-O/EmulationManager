using System.Diagnostics;
using EmulationManager.Models;

namespace EmulationManager.LaunchStrategies;

public sealed class DirectLaunchStrategy : ILaunchStrategy
{
    public LaunchMethod Method => LaunchMethod.Direct;

    public Task<Process> LaunchAsync(
        LaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var startInfo = new ProcessStartInfo
        {
            FileName = request.Emulator.ExecutablePath,

            WorkingDirectory =
                Path.GetDirectoryName(
                    request.Emulator.ExecutablePath)!,

            UseShellExecute = false
        };

        startInfo.ArgumentList.Add(request.RomPath);

        Process? process = Process.Start(startInfo);

        return Task.FromResult(
            process ??
            throw new InvalidOperationException(
                "Windows did not return an emulator process."));
    }

    private static void Validate(LaunchRequest request)
    {
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
