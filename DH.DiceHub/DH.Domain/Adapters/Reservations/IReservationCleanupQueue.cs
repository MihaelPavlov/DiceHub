using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.Reservations;

public interface IReservationCleanupQueue : QueueBase
{
    Task AddReservationCleaningJob(int reservationId, ReservationType type, DateTime removingTime);
    Task UpdateReservationCleaningJob(int reservationId, ReservationType type, DateTime newRemovingTime);
    Task<List<QueuedJob>> TryDequeue(CancellationToken cancellationToken);
    Task CancelReservationCleaningJob(int reservationId, ReservationType type);
}
