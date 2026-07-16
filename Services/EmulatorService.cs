using System.Diagnostics;
using EmulationManager.Configuration;
using EmulationManager.LaunchStrategies;
using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class EmulatorService
{
    private readonly AppPaths paths;
    private readonly LaunchStrategyFactory strategyFactory;

    public EmulatorService(
        AppPaths paths,
        LaunchStrategyFactory strategyFactory)
    {
        this.paths = paths;
        this.strategyFactory = strategyFactory;
    }

    public EmulatorBuild EdenStable =>
        new()
        {
            Name = "Eden Stable",
            ExecutablePath = paths.EdenStablePath
        };

    public EmulatorBuild EdenNightly =>
        new()
        {
            Name = "Eden Nightly",
            ExecutablePath = paths.EdenNightlyPath
        };

    public Task<Process> LaunchAsync(
        EmulatorBuild emulator,
        string romPath,
        LaunchMethod method)
    {
        if (method == LaunchMethod.Ask)
        {
            throw new InvalidOperationException(
                "Ask must be resolved by the user interface before launching.");
        }

        ILaunchStrategy strategy =
            strategyFactory.Get(method, emulator);

        var request = new LaunchRequest
        {
            Emulator = emulator,
            RomPath = romPath,

            DisplayName =
                $"{Path.GetFileNameWithoutExtension(romPath)} - " +
                emulator.Name
        };

        return strategy.LaunchAsync(request);
    }
}
