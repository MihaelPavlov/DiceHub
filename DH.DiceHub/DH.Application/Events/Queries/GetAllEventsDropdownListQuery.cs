using DH.Domain.Entities;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetAllEventsDropdownListQuery : IRequest<List<GetAllEventsDropdownListModel>>;

internal class GetAllEventsDropdownListQueryHandler(IRepository<Event> repository) : IRequestHandler<GetAllEventsDropdownListQuery, List<GetAllEventsDropdownListModel>>
{
    readonly IRepository<Event> repository = repository;

    public async Task<List<GetAllEventsDropdownListModel>> Handle(GetAllEventsDropdownListQuery request, CancellationToken cancellationToken)
    {
        return await this.repository.GetWithPropertiesAsync(x => new GetAllEventsDropdownListModel
        {
            Id = x.Id,
            Name = x.Name
        }, cancellationToken);
    }
}

public class GetAllEventsDropdownListModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}