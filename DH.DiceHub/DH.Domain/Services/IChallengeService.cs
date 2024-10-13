using DH.Domain.Entities;

namespace DH.Domain.Services;

public interface IChallengeService : IDomainService<Challenge>
{
    Task<int> Create(Challenge challenge, CancellationToken cancellationToken);
}
