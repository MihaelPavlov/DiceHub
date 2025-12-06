using DH.Domain.Enums;

namespace DH.Domain.Adapters.Reservations;

public static class ReservationCleanupHelper
{
    public static string BuildJobId(int ReservationId, ReservationType type) => $"{ReservationId}-{type.ToString()}";
}
