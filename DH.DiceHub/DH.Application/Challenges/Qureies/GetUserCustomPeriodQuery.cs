using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserCustomPeriodQuery : IRequest<GetUserCustomPeriodQueryModel>;

internal class GetUserCustomPeriodQueryHandler(IChallengeService challengeService) : IRequestHandler<GetUserCustomPeriodQuery, GetUserCustomPeriodQueryModel>
{
    readonly IChallengeService challengeService = challengeService;

    public async Task<GetUserCustomPeriodQueryModel> Handle(GetUserCustomPeriodQuery request, CancellationToken cancellationToken)
    {
        return await this.challengeService.GetUserCustomPeriodData(cancellationToken);
    }
}
