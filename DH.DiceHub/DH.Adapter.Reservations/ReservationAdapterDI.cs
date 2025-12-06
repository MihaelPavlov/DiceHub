using DH.Domain.Adapters.Reservations;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Reservations;

public static class ReservationAdapterDI
{
    public static IServiceCollection AddReservationAdapter(
        this IServiceCollection services)
        => services
            .AddScoped<IReservationCleanupQueue, ReservationCleanupQueue>()
            .AddHostedService<ReservationCleanupWorker>();
}
