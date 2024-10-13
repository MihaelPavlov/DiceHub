using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class RewardService : IRewardService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;

    public RewardService(IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
    }

    public async Task<int> CreateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    reward.CreatedBy = this.userContext.UserId;
                    await context.ChallengeRewards.AddAsync(reward, cancellationToken);

                    await context.ChallengeRewardImages
                        .AddAsync(new ChallengeRewardImage
                        {
                            Reward = reward,
                            FileName = fileName,
                            ContentType = contentType,
                            Data = imageStream.ToArray(),
                        }, cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return reward.Id;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }

    public async Task UpdateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        using (var context = await this._contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbReward = await context.ChallengeRewards
                .AsTracking()
                .Include(g => g.Image)
                .FirstOrDefaultAsync(x => x.Id == reward.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(ChallengeReward), reward.Id);

            var oldImage = dbReward.Image;

            dbReward.Name = reward.Name;
            dbReward.Description = reward.Description;
            dbReward.RequiredPoints = reward.RequiredPoints;
            dbReward.Level = reward.Level;
            dbReward.UpdatedDate = DateTime.UtcNow;
            dbReward.UpdatedBy = this.userContext.UserId;

            var newRewardImage = new ChallengeRewardImage
            {
                FileName = fileName,
                ContentType = contentType,
                Data = imageStream.ToArray(),
            };

            dbReward.Image = newRewardImage;

            context.ChallengeRewardImages.Remove(oldImage);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}