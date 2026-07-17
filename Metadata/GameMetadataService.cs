using EmulationManager.Models;

namespace EmulationManager.Metadata;

public sealed class GameMetadataService
{
    private readonly IReadOnlyList<IGameMetadataProvider> providers;

    public GameMetadataService(
        IEnumerable<IGameMetadataProvider> providers)
    {
        this.providers = providers.ToList();
    }

    public async Task<GameMetadata?> ReadAsync(
        GameFile game,
        CancellationToken cancellationToken = default)
    {
        foreach (IGameMetadataProvider provider in providers)
        {
            if (!provider.CanRead(game))
            {
                continue;
            }

            GameMetadata? metadata =
                await provider.ReadAsync(game, cancellationToken);

            if (metadata is not null)
            {
                return metadata;
            }
        }

        return null;
    }
}
