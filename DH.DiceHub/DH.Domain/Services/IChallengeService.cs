using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;

namespace DH.Domain.Services;

public interface IChallengeService : IDomainService<Challenge>
{
    Task<int> Create(Challenge challenge, CancellationToken cancellationToken);
    Task<List<GetUserChallengeListQueryModel>> GetUserChallenges(CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
}
