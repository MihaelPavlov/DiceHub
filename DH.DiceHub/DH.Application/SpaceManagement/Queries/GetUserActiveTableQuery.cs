using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.SpaceManagement.Queries;

public record GetUserActiveTableQuery : IRequest<GetUserActiveTableQueryModel>;

internal class GetUserActiveTableQueryHandler : IRequestHandler<GetUserActiveTableQuery, GetUserActiveTableQueryModel>
{
    readonly ILogger<GetUserActiveTableQuery> logger;
    readonly IRepository<SpaceTable> spaceTableRepository;
    readonly IRepository<SpaceTableParticipant> spaceTableParticipantRepository;
    readonly IUserContext userContext;

    public GetUserActiveTableQueryHandler(IRepository<SpaceTable> spaceTableRepository, IRepository<SpaceTableParticipant> spaceTableParticipantRepository, IUserContext userContext, ILogger<GetUserActiveTableQuery> logger)
    {
        this.spaceTableRepository = spaceTableRepository;
        this.spaceTableParticipantRepository = spaceTableParticipantRepository;
        this.userContext = userContext;
        this.logger = logger;
    }

    public async Task<GetUserActiveTableQueryModel> Handle(GetUserActiveTableQuery request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.spaceTableRepository
            .GetByAsync(x =>
                x.CreatedBy == this.userContext.UserId &&
                x.IsTableActive, cancellationToken);

        var spaceTableParticipations = await this.spaceTableParticipantRepository
            .GetWithPropertiesAsync(
                x => x.UserId == this.userContext.UserId && x.SpaceTable.IsTableActive,
                x => new
                {
                    SpaceTableId = x.SpaceTableId,
                    SpaceTableName = x.SpaceTable.Name
                },
                cancellationToken);

        var userActiveTableResult = new GetUserActiveTableQueryModel();

        if (spaceTable != null)
        {
            userActiveTableResult.IsPlayerHaveActiveTable = true;
            userActiveTableResult.ActiveTableName = spaceTable.Name;
            userActiveTableResult.ActiveTableId = spaceTable.Id;
        }
        else if (spaceTableParticipations.Count == 1)
        {
            var spaceTableFromParticipant = spaceTableParticipations.First();
            userActiveTableResult.IsPlayerParticipateInTable = true;
            userActiveTableResult.ActiveTableName = spaceTableFromParticipant.SpaceTableName;
            userActiveTableResult.ActiveTableId = spaceTableFromParticipant.SpaceTableId;
        }
        else if (spaceTableParticipations.Count > 1)
        {
            this.logger.LogWarning("Current user {userId}, participate in {tableParticipationsCount} tables. Allowed is 1", this.userContext.UserId, spaceTableParticipations.Count);
        }

        return userActiveTableResult;
    }
}