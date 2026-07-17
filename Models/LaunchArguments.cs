namespace EmulationManager.Models;

public sealed class LaunchArguments
{
    public string? RomPath { get; init; }

    public string? GameId { get; init; }

    public static bool TryParse(
        string[] args,
        out LaunchArguments? result)
    {
        result = null;

        if (args.Length < 2)
        {
            return false;
        }

        for (int index = 0;
             index < args.Length - 1;
             index++)
        {
            string option = args[index];
            string value = args[index + 1];

            if (option.Equals(
                    "--launch",
                    StringComparison.OrdinalIgnoreCase))
            {
                result = new LaunchArguments
                {
                    RomPath = value
                };

                return true;
            }

            if (option.Equals(
                    "--game-id",
                    StringComparison.OrdinalIgnoreCase))
            {
                result = new LaunchArguments
                {
                    GameId = value
                };

                return true;
            }
        }

        return false;
    }
}