using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Repositories;
using DH.Domain.Entities;
using DH.Domain.Adapters.Authentication.Models.Enums;

internal class RewardQRCodeState : IQRCodeState
{
    readonly IUserContext userContext;
    readonly IRepository<UserChallengeReward> userChallengeRewardRepository;

    public RewardQRCodeState(IUserContext userContext, IRepository<UserChallengeReward> userChallengeRewardRepository)
    {
        this.userContext = userContext;
        this.userChallengeRewardRepository = userChallengeRewardRepository;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var result = new QrCodeValidationResult(data.Id, QrCodeType.Reward);
        if (this.userContext.RoleKey == (int)Role.User)
            return SetError(result, "Access denied: only authorized staff can give rewards.");

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return SetError(result, "Invalid request: User ID is missing.");

        var userReward = await this.userChallengeRewardRepository
             .GetByAsync(x => x.Id == data.Id && x.UserId == userId, cancellationToken);

        if (userReward == null)
            return SetError(result, "No reward found for the specified user.");

        if (userReward.IsExpired)
            return SetError(result, "This reward has expired and can no longer be claimed.");

        if (userReward.IsClaimed)
            return SetError(result, "This reward has already been claimed.");

        result.IsValid = true;
        return result;
    }

    private QrCodeValidationResult SetError(QrCodeValidationResult result, string errorMessage)
    {
        result.ErrorMessage = errorMessage;
        result.IsValid = false;
        return result;
    }
}