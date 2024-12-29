using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetRewardImageByIdQuery(int Id) : IRequest<RewardImageResult>;

internal class GetRewardImageByIdQueryHandler : IRequestHandler<GetRewardImageByIdQuery, RewardImageResult>
{
    readonly IRepository<ChallengeRewardImage> repository;

    public GetRewardImageByIdQueryHandler(IRepository<ChallengeRewardImage> repository)
    {
        this.repository = repository;
    }

    public async Task<RewardImageResult> Handle(GetRewardImageByIdQuery request, CancellationToken cancellationToken)
    {
        var rewardImage = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ChallengeRewardImage));

        return new RewardImageResult(rewardImage.FileName, rewardImage.ContentType, rewardImage.Data);
    }
}

public record RewardImageResult(string FileName, string ContentType, byte[] Data);
