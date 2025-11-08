using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.SpaceManagementModels.Queries;

namespace DH.Domain.Services;

public interface ISpaceTableService : IDomainService<SpaceTable>
{
    Task<int> Create(SpaceTable spaceTable, CancellationToken cancellationToken, bool fromGameReservation = false, string userId = "");
    Task<List<GetSpaceTableReservationHistoryQueryModel>> GetSpaceTableReservationListByStatus(ReservationStatus? status, CancellationToken cancellationToken);
    Task<int> GetActiveSpaceTableReservationsCount(CancellationToken cancellationToken);
    Task CloseActiveTables(CancellationToken cancellationToken);
}
