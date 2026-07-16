namespace EmulationManager.Models;

public sealed class LaunchArguments
{
    public required string RomPath { get; init; }

    public static bool TryParse(
        string[] args,
        out LaunchArguments? result)
    {
        result = null;

        if (args.Length < 2)
        {
            return false;
        }

        if (!args[0].Equals(
                "--launch",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(args[1]))
        {
            return false;
        }

        result = new LaunchArguments
        {
            RomPath = Path.GetFullPath(args[1])
        };

        return true;
    }
}
