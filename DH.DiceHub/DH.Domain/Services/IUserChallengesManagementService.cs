namespace DH.Domain.Services;

public interface IUserChallengesManagementService
{
    Task InitiateNewUserChallenges(string userId, CancellationToken cancellationToken);

    Task AddChallengeToUser(string userId, CancellationToken cancellationToken);
}
