using EmulationManager.Models;

namespace EmulationManager.Models;

public sealed class LaunchRequest
{
    public required EmulatorBuild Emulator { get; init; }

    public required string RomPath { get; init; }

    public required string DisplayName { get; init; }
}
