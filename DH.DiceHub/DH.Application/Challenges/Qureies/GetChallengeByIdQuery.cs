using DH.Domain.Entities;
using DH.OperationResultCore.Exceptions;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetChallengeByIdQuery(int Id) : IRequest<GetChallengeByIdQueryModel>;

internal class GetChallengeByIdQueryHandler : IRequestHandler<GetChallengeByIdQuery, GetChallengeByIdQueryModel>
{
    readonly IRepository<Challenge> repository;

    public GetChallengeByIdQueryHandler(IRepository<Challenge> repository)
    {
        this.repository = repository;
    }

    public async Task<GetChallengeByIdQueryModel> Handle(GetChallengeByIdQuery request, CancellationToken cancellationToken)
    {
        var challenge = await this.repository.GetWithPropertiesAsync(
        x => x.Id == request.Id,
        x => new GetChallengeByIdQueryModel
        {
            Id = x.Id,
            RewardPoints = x.RewardPoints,
            GameId = x.GameId,
            Attempts = x.Attempts,
        }, cancellationToken);

        return challenge.FirstOrDefault()
            ?? throw new NotFoundException(nameof(Challenge), request.Id);
    }
}