using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetGameDropdownListQuery : IRequest<List<GetGameDropdownListQueryModel>>;

internal class GetGameDropdownListQueryHandler : IRequestHandler<GetGameDropdownListQuery, List<GetGameDropdownListQueryModel>>
{
    readonly IRepository<Game> repository;

    public GetGameDropdownListQueryHandler(IRepository<Game> repository)
    {
        this.repository = repository;
    }

    public async Task<List<GetGameDropdownListQueryModel>> Handle(GetGameDropdownListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync<GetGameDropdownListQueryModel>(x => !x.IsDeleted, x => new(x.Id, x.Name), cancellationToken);
    }
}