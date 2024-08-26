using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameInventoryQuery(int Id) : IRequest<GetGameInvetoryQueryModel>;

internal class GetGameInventoryQueryHandler : IRequestHandler<GetGameInventoryQuery, GetGameInvetoryQueryModel>
{
    readonly IRepository<GameInventory> repository;

    public GetGameInventoryQueryHandler(IRepository<GameInventory> repository)
    {
        this.repository = repository;
    }

    public async Task<GetGameInvetoryQueryModel> Handle(GetGameInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventory = await this.repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(GameInventory));

        return new GetGameInvetoryQueryModel(inventory.TotalCopies, inventory.AvailableCopies);
    }
}