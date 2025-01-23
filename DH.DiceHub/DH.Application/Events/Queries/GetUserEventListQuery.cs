using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetUserEventListQuery : IRequest<List<GetEventListQueryModel>>;

internal class GetUserEventListQueryHandler(IEventService eventService) : IRequestHandler<GetUserEventListQuery, List<GetEventListQueryModel>>
{
    readonly IEventService eventService = eventService;

    public async Task<List<GetEventListQueryModel>> Handle(GetUserEventListQuery request, CancellationToken cancellationToken)
    {
        return await this.eventService.GetUserEvents(cancellationToken);
    }
}