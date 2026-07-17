using System.Text.RegularExpressions;
using EmulationManager.Models;

namespace EmulationManager.Metadata;

public sealed partial class FilenameSwitchMetadataProvider
    : IGameMetadataProvider
{
    public bool CanRead(GameFile game)
    {
        return game.ConsoleName.Equals(
            "Switch",
            StringComparison.OrdinalIgnoreCase);
    }

    public Task<GameMetadata?> ReadAsync(
        GameFile game,
        CancellationToken cancellationToken = default)
    {
        Match match = SwitchTitleIdRegex().Match(game.FileName);

        if (!match.Success)
        {
            return Task.FromResult<GameMetadata?>(null);
        }

        var metadata = new GameMetadata
        {
            TitleId = match.Groups[1].Value.ToUpperInvariant(),
            DisplayName = game.GameName,
            WasReadFromFile = false
        };

        return Task.FromResult<GameMetadata?>(metadata);
    }

    [GeneratedRegex(
        @"\[([0-9A-Fa-f]{16})\]",
        RegexOptions.CultureInvariant)]
    private static partial Regex SwitchTitleIdRegex();
}