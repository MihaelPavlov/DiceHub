using DH.Application.Cqrs;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;

namespace DH.Application.Games.Queries;

public record GetGameListQuery : ICommand<List<GetGameListQueryModel>>;

internal class GetGameListQueryHandler : AbstractCommandHandler<GetGameListQuery, List<GetGameListQueryModel>>
{
    readonly IRepository<Game> gameRepository;

    public GetGameListQueryHandler(IRepository<Game> gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    protected override async Task<List<GetGameListQueryModel>> HandleAsync(GetGameListQuery request, CancellationToken cancellationToken)
    {
        return await gameRepository.GetWithPropertiesAsync(x => new GetGameListQueryModel
        {
            Id = x.Id,
            Description = x.Description,
            Name = x.Name,
            Likes = x.Likes,
            ImageUrl = x.ImageUrl
        }, cancellationToken);
    }
}
