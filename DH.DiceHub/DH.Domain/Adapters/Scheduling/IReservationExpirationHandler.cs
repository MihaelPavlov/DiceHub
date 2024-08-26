using DH.Domain.Entities;

namespace DH.Domain.Adapters.Scheduling;

public interface IReservationExpirationHandler
{
    Task<bool> ProcessReservationExpirationAsync(int reservationId, CancellationToken cancellationToken);
    Task ProcessFailedReservationExpirationAsync(string data,string errorMessage, CancellationToken cancellationToken);
}