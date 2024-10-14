using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceActivityStatsQuery : IRequest<GetSpaceActivityStatsQueryModel>;

internal class GetSpaceActivityStatsQueryHandler : IRequestHandler<GetSpaceActivityStatsQuery, GetSpaceActivityStatsQueryModel>
{
    readonly ILogger<GetUserActiveTableQuery> logger;
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;

    public GetSpaceActivityStatsQueryHandler(IRepository<SpaceTable> spaceTableRepository, IRepository<SpaceTableParticipant> spaceTableParticipantRepository, ILogger<GetUserActiveTableQuery> logger)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.logger = logger;
    }

    public async Task<GetSpaceActivityStatsQueryModel> Handle(GetSpaceActivityStatsQuery request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository.GetWithPropertiesAsync(x => x.IsTableActive, x => x, cancellationToken);
        var spaceTableParticipation = await this.spaceTableParticipantRepository.GetWithPropertiesAsync(x => x.SpaceTable.IsTableActive, x => x, cancellationToken);

        var result = new GetSpaceActivityStatsQueryModel();
        result.TotalActiveTables = spaceTable.Count;
        result.TotalPeopleInSpace = spaceTableParticipation.Count + spaceTable.Count;

        return result;
    }
}