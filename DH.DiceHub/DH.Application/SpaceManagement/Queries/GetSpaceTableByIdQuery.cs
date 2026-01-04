using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Queries;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetSpaceTableByIdQuery(int Id) : IRequest<GetSpaceTableByIdQueryModel>;

internal class GetSpaceTableByIdQueryHandler(IRepository<SpaceTable> repository, IRepository<Game> gameRepository) : IRequestHandler<GetSpaceTableByIdQuery, GetSpaceTableByIdQueryModel>
{
    public IRepository<SpaceTable> repository = repository;
    public IRepository<Game> gameRepository = gameRepository;

    public async Task<GetSpaceTableByIdQueryModel> Handle(GetSpaceTableByIdQuery request, CancellationToken cancellationToken)
    {
        var spaceTable = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTable), request.Id);

        var mappedTalbe = spaceTable.Adapt<GetSpaceTableByIdQueryModel>();

        var game = await this.gameRepository.GetByAsync(x => x.Id == spaceTable.GameId, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), spaceTable.GameId);

        mappedTalbe.GameName = game.Name;

        return mappedTalbe;
    }
}