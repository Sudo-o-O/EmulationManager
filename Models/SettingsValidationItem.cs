namespace EmulationManager.Models;

public sealed class SettingsValidationItem
{
    public required string Name { get; init; }

    public required bool IsValid { get; init; }

    public required string Message { get; init; }

    public bool IsOptional { get; init; }
}