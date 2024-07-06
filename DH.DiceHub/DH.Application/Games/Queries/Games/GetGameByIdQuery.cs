using DH.Application.Cqrs;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;

namespace DH.Application.Games.Queries.Games;

public record GetGameByIdQuery(int Id) : ICommand<GetGameByIdQueryModel>;

internal class GetGameByIdQueryHandler : AbstractCommandHandler<GetGameByIdQuery, GetGameByIdQueryModel>
{
    readonly IRepository<Game> repository;

    public GetGameByIdQueryHandler(IRepository<Game> repository)
    {
        this.repository = repository;
    }
    protected override async Task<GetGameByIdQueryModel> HandleAsync(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await repository.GetByAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Game), request.Id);

        return new GetGameByIdQueryModel
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            AveragePlaytime = game.AveragePlaytime,
            ImageUrl = game.ImageUrl,
            Likes = game.Likes,
            MinAge = game.MinAge,
            MaxPlayers = game.MaxPlayers,
            MinPlayers = game.MinPlayers,
        };
    }
}
