using DH.Domain.Models.EventModels.Queries;
using DH.Domain.Services;
using MediatR;

namespace DH.Application.Events.Queries;

public record GetEventListQuery(string? SearchExpression) : IRequest<List<GetEventListQueryModel>>;

internal class GetEventListQueryHandler : IRequestHandler<GetEventListQuery, List<GetEventListQueryModel>>
{
    readonly IEventService eventService;

    public GetEventListQueryHandler(IEventService eventService)
    {
        this.eventService = eventService;
    }

    public async Task<List<GetEventListQueryModel>> Handle(GetEventListQuery request, CancellationToken cancellationToken)
    {
        return await this.eventService.GetListBySearchExpressionAsync(request.SearchExpression ?? string.Empty, cancellationToken);
    }
}