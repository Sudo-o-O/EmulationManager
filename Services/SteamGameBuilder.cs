using System.Security.Cryptography;
using System.Text;
using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class SteamGameBuilder
{
    private readonly string managerExecutablePath;

    public SteamGameBuilder()
    {
        managerExecutablePath =
            Environment.ProcessPath
            ?? throw new InvalidOperationException(
                "The Emulation Manager executable path could not be determined.");
    }

    public IReadOnlyList<SteamGameEntry> Build(
        IReadOnlyCollection<GameFile> files)
    {
        return files
            .Where(file =>
                file.ContentType.Equals(
                    "Base Game",
                    StringComparison.OrdinalIgnoreCase) ||
                file.ContentType.Equals(
                    "Game",
                    StringComparison.OrdinalIgnoreCase))
            .GroupBy(
                file => new
                {
                    file.ConsoleName,
                    file.GameName
                })
            .Select(group =>
            {
                GameFile game =
                    group.First();

                return new SteamGameEntry
                {
                    Id = CreateStableId(
                        game.ConsoleName,
                        game.GameName),

                    Title = game.GameName,
                    ConsoleName = game.ConsoleName,
                    RomPath = game.FullPath,

                    ManagerExecutablePath =
                        managerExecutablePath
                };
            })
            .OrderBy(game => game.ConsoleName)
            .ThenBy(game => game.Title)
            .ToList();
    }

    private static string CreateStableId(
        string consoleName,
        string gameName)
    {
        string source =
            $"{consoleName.Trim().ToUpperInvariant()}|" +
            gameName.Trim().ToUpperInvariant();

        byte[] hash =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(source));

        return Convert.ToHexString(hash)[..16];
    }
}
