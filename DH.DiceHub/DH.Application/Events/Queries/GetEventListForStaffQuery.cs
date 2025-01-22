using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventListForStaffQuery(string? SearchExpression) : IRequest<List<GetEventListQueryModel>>;

internal class GetEventListForStaffQueryHandler(IEventService eventService) : IRequestHandler<GetEventListForStaffQuery, List<GetEventListQueryModel>>
{
    readonly IEventService eventService = eventService;

    public async Task<List<GetEventListQueryModel>> Handle(GetEventListForStaffQuery request, CancellationToken cancellationToken)
    {
        return await this.eventService.GetListForStaff(request.SearchExpression ?? string.Empty, cancellationToken);
    }
}