using DH.Domain.Entities;
using DH.Domain.Repositories;

namespace DH.Adapter.Authentication.Repositories;

public class TestRepository : ITestRepository
{
    readonly AppIdentityDbContext dBContext;
    public TestRepository(AppIdentityDbContext dBContext)
    {
        this.dBContext = dBContext;
    }

    public async Task<int> CreateAsync(Test entity, CancellationToken cancellationToken)
    {
        var game = await this.dBContext.AddAsync(entity, cancellationToken);
        await this.dBContext.SaveChangesAsync(cancellationToken);

        return game.Entity.Id;
    }
}
