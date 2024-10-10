using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Repositories;
using Mapster;
using MediatR;

namespace DH.Application.Challenges.Qureies;

public record GetUserChallengePeriodPerformanceQuery : IRequest<GetUserChallengePeriodPerformanceQueryModel>;

internal class GetUserChallengePeriodPerformanceQueryHandler : IRequestHandler<GetUserChallengePeriodPerformanceQuery, GetUserChallengePeriodPerformanceQueryModel>
{
    readonly IRepository<UserChallengePeriodPerformance> repository;
    readonly IUserContext userContext;

    public GetUserChallengePeriodPerformanceQueryHandler(IRepository<UserChallengePeriodPerformance> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<GetUserChallengePeriodPerformanceQueryModel> Handle(GetUserChallengePeriodPerformanceQuery request, CancellationToken cancellationToken)
    {
        var period = await this.repository.GetByAsync(x => x.UserId == this.userContext.UserId && x.IsPeriodActive, cancellationToken);
        return period.Adapt<GetUserChallengePeriodPerformanceQueryModel>();
    }
}