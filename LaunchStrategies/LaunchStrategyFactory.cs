using EmulationManager.Models;

namespace EmulationManager.LaunchStrategies;

public sealed class LaunchStrategyFactory
{
    private readonly Dictionary<LaunchMethod, ILaunchStrategy> strategies;

    public LaunchStrategyFactory(
        IEnumerable<ILaunchStrategy> strategies)
    {
        this.strategies =
            strategies.ToDictionary(
                strategy => strategy.Method);
    }

    public ILaunchStrategy Get(
        LaunchMethod requestedMethod,
        EmulatorBuild emulator)
    {
        LaunchMethod resolvedMethod =
            ResolveAutomaticMethod(
                requestedMethod,
                emulator);

        if (!strategies.TryGetValue(
                resolvedMethod,
                out ILaunchStrategy? strategy))
        {
            throw new InvalidOperationException(
                $"No launch strategy is registered for {resolvedMethod}.");
        }

        return strategy;
    }

    private static LaunchMethod ResolveAutomaticMethod(
        LaunchMethod requestedMethod,
        EmulatorBuild emulator)
    {
        if (requestedMethod != LaunchMethod.Auto)
        {
            return requestedMethod;
        }

        if (OperatingSystem.IsWindows() &&
            emulator.Name.StartsWith(
                "Eden",
                StringComparison.OrdinalIgnoreCase))
        {
            return LaunchMethod.Detached;
        }

        return LaunchMethod.Direct;
    }
}
