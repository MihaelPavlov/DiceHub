using DH.Domain.Enums;
using DH.Domain.Queue;

namespace DH.Domain.Adapters.Reservations;

public record ReservationCleanupJobInfo(
    int ReservationId, ReservationType Type, DateTime RemovingTime)
        : JobInfoBase(ReservationCleanupHelper.BuildJobId(ReservationId, Type));
