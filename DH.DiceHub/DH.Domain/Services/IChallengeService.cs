using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Models.ChallengeModels.Queries;

namespace DH.Domain.Services;

public interface IChallengeService : IDomainService<Challenge>
{
    Task<int> Create(Challenge challenge, CancellationToken cancellationToken);
    Task<List<GetUserChallengeListQueryModel>> GetUserChallenges(CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
    Task SaveCustomPeriod(SaveCustomPeriodDto customPeriod, CancellationToken cancellationToken);
    Task<GetUserCustomPeriodQueryModel> GetUserCustomPeriodData(CancellationToken cancellationToken);
    Task<List<GetUserUniversalChallengeListQueryModel>> GetUserUniversalChallenges(CancellationToken cancellationToken);
}
