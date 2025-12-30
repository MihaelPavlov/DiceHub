using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Entities;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class RewardService : IRewardService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;
    readonly IFileManagerClient fileManagerClient;

    public RewardService(
        IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext, IFileManagerClient fileManagerClient)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
        this.fileManagerClient = fileManagerClient;
    }

    public async Task<int> CreateReward(ChallengeReward reward, string fileName, string contentType, MemoryStream imageStream, CancellationToken cancellationToken)
    {
        var imageUrl = await this.fileManagerClient.UploadFileAsync(
            FileManagerFolders.Rewards.ToString(), fileName, imageStream.ToArray());
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    reward.CreatedBy = this.userContext.UserId!;
                    reward.ImageUrl = imageUrl;
                    await context.ChallengeRewards.AddAsync(reward, cancellationToken);

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

    public async Task UpdateReward(ChallengeReward reward, string? fileName, string? contentType, MemoryStream? imageStream, CancellationToken cancellationToken)
    {
        string? currenFileImageUrl = null;

        if (!string.IsNullOrEmpty(fileName))
            currenFileImageUrl = this.fileManagerClient.GetPublicUrl(
                FileManagerFolders.Games.ToString(), fileName);

        using (var context = await this._contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var dbReward = await context.ChallengeRewards
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == reward.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(ChallengeReward), reward.Id);

            dbReward.Name_EN = reward.Name_EN;
            dbReward.Name_BG = reward.Name_BG;
            dbReward.Description_EN = reward.Description_EN;
            dbReward.Description_BG = reward.Description_BG;
            dbReward.CashEquivalent = reward.CashEquivalent;
            dbReward.RequiredPoints = reward.RequiredPoints;
            dbReward.Level = reward.Level;
            dbReward.UpdatedDate = DateTime.UtcNow;
            dbReward.UpdatedBy = this.userContext.UserId!;
            if (currenFileImageUrl != null && currenFileImageUrl != dbReward.ImageUrl && imageStream != null)
            {
                var imageUrl = await this.fileManagerClient.UploadFileAsync(
                    FileManagerFolders.Rewards.ToString(), fileName!, imageStream.ToArray());
                dbReward.ImageUrl = imageUrl;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}