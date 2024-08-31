using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventByIdQuery(int Id) : IRequest<GetEventByIdQueryModel>;

internal class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdQueryModel>
{
    readonly IEventService eventService;

    public GetEventByIdQueryHandler(IEventService eventService)
    {
        this.eventService = eventService;
    }

    public async Task<GetEventByIdQueryModel> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        return await this.eventService.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), request.Id);
    }
}
