using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventListForUserQuery : IRequest<List<GetEventListQueryModel>>;

internal class GetEventListForUserQueryHandler(IEventService eventService) : IRequestHandler<GetEventListForUserQuery, List<GetEventListQueryModel>>
{
    readonly IEventService eventService = eventService;

    public async Task<List<GetEventListQueryModel>> Handle(GetEventListForUserQuery request, CancellationToken cancellationToken)
    {
        return await this.eventService.GetListForUsers(cancellationToken);
    }
}