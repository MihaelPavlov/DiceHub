using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserCustomPeriodQuery : IRequest<GetUserCustomPeriodQueryModel>;

internal class GetUserCustomPeriodQueryHandler : IRequestHandler<GetUserCustomPeriodQuery, GetUserCustomPeriodQueryModel>
{
    readonly IChallengeService challengeService;

    public GetUserCustomPeriodQueryHandler(
        IChallengeService challengeService)
    {
        this.challengeService = challengeService;
    }

    public async Task<GetUserCustomPeriodQueryModel> Handle(GetUserCustomPeriodQuery request, CancellationToken cancellationToken)
    {
        return await this.challengeService.GetUserCustomPeriodData(cancellationToken);
    }
}
