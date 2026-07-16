namespace EmulationManager.Models;

public sealed class EmulatorBuild
{
    public required string Name { get; init; }

    public required string ExecutablePath { get; init; }

    public bool IsInstalled =>
        File.Exists(ExecutablePath);
}
