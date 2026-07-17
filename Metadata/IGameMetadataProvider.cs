using EmulationManager.Models;

namespace EmulationManager.Metadata;

public interface IGameMetadataProvider
{
    bool CanRead(GameFile game);

    Task<GameMetadata?> ReadAsync(
        GameFile game,
        CancellationToken cancellationToken = default);
}