using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed partial class GameIdService
{
    public string CreateId(GameFile game)
    {
        if (game.ConsoleName.Equals(
                "Switch",
                StringComparison.OrdinalIgnoreCase))
        {
            string? switchTitleId =
                ExtractSwitchTitleId(game.FileName);

            if (switchTitleId is not null)
            {
                return $"switch:{switchTitleId}";
            }
        }

        return CreateFallbackId(
            game.ConsoleName,
            game.GameName);
    }

    private static string? ExtractSwitchTitleId(
        string fileName)
    {
        Match match =
            SwitchTitleIdRegex().Match(fileName);

        if (!match.Success)
        {
            return null;
        }

        return match.Groups[1]
            .Value
            .ToUpperInvariant();
    }

    private static string CreateFallbackId(
        string consoleName,
        string gameName)
    {
        string source =
            $"{consoleName.Trim().ToUpperInvariant()}|" +
            gameName.Trim().ToUpperInvariant();

        byte[] hash =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(source));

        return
            $"{Normalize(consoleName)}:" +
            Convert.ToHexString(hash)[..16];
    }

    private static string Normalize(string value)
    {
        return new string(
            value.Trim()
                .ToLowerInvariant()
                .Where(character =>
                    char.IsLetterOrDigit(character))
                .ToArray());
    }

    [GeneratedRegex(
        @"\[([0-9A-Fa-f]{16})\]",
        RegexOptions.CultureInvariant)]
    private static partial Regex SwitchTitleIdRegex();
}