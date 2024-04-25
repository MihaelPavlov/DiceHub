using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Data.Repositories;

public class GameRepository : IGameRepository
{
    readonly MyDbContext dBContext;
    public GameRepository(MyDbContext dBContext)
    {
        this.dBContext = dBContext;
    }

    public async Task<int> CreateAsync(Game entity, CancellationToken cancellationToken)
    {
        var game = await this.dBContext.AddAsync(entity, cancellationToken);
        await this.dBContext.SaveChangesAsync(cancellationToken);

        return game.Entity.Id;
    }
}
