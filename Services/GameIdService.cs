using System.Security.Cryptography;
using System.Text;
using EmulationManager.Metadata;
using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class GameIdService
{
    private readonly GameMetadataService metadataService;

    public GameIdService(
        GameMetadataService metadataService)
    {
        this.metadataService = metadataService;
    }

    public async Task<string> CreateIdAsync(
        GameFile game,
        CancellationToken cancellationToken = default)
    {
        GameMetadata? metadata =
            await metadataService.ReadAsync(
                game,
                cancellationToken);

        if (!string.IsNullOrWhiteSpace(metadata?.TitleId))
        {
            return
                $"{Normalize(game.ConsoleName)}:" +
                metadata.TitleId.ToUpperInvariant();
        }

        return CreateFallbackId(
            game.ConsoleName,
            game.GameName);
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
                .Where(char.IsLetterOrDigit)
                .ToArray());
    }
}