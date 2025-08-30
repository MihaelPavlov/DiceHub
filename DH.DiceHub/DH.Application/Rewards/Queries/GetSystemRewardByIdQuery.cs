using DH.Domain.Entities;
using DH.Domain.Models.RewardModels.Queries;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Rewards.Queries;

public record GetSystemRewardByIdQuery(int Id) : IRequest<GetRewardByIdQueryModel>;

internal class GetSystemRewardByIdQueryHandler : IRequestHandler<GetSystemRewardByIdQuery, GetRewardByIdQueryModel>
{
    readonly IRepository<ChallengeReward> repository;

    public GetSystemRewardByIdQueryHandler(IRepository<ChallengeReward> repository)
    {
        this.repository = repository;
    }

    public async Task<GetRewardByIdQueryModel> Handle(GetSystemRewardByIdQuery request, CancellationToken cancellationToken)
    {
        var rewards = await this.repository.GetWithPropertiesAsync(
            x => !x.IsDeleted && x.Id == request.Id,
            x => new GetRewardByIdQueryModel
            {
                Id = x.Id,
                Description_EN = x.Description_EN,
                Description_BG = x.Description_BG,
                CashEquivalent = x.CashEquivalent,
                ImageId = x.Image.Id,
                RequiredPoints = x.RequiredPoints,
                Level = x.Level,
                Name_EN = x.Name_EN,
                Name_BG = x.Name_BG
            }, cancellationToken);

        return rewards.FirstOrDefault()
            ?? throw new NotFoundException(nameof(ChallengeReward), request.Id);
    }
}
