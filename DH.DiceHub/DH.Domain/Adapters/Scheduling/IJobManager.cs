namespace DH.Domain.Adapters.Scheduling;

public interface IJobManager
{
    Task CreateReservationJob(int reservationId, DateTime reservationTime, int durationInMinutes);
}
