using DH.Domain.Services;
using MediatR;

namespace DH.Application.SpaceManagement.Queries;

public record GetActiveSpaceTableReservationCountQuery : IRequest<int>;

internal class GetActiveSpaceTableReservationCountQueryHandler(ISpaceTableService spaceTableService) : IRequestHandler<GetActiveSpaceTableReservationCountQuery, int>
{
    readonly ISpaceTableService spaceTableService = spaceTableService;

    public async Task<int> Handle(GetActiveSpaceTableReservationCountQuery request, CancellationToken cancellationToken)
    {
        return await this.spaceTableService.GetActiveSpaceTableReservationsCount(cancellationToken);
    }
}