using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record ApproveSpaceTableReservationCommand(int ReservationId, string InternalNote, string PublicNote) : IRequest;

internal class ApproveSpaceTableReservationCommandHandler(
    IRepository<SpaceTableReservation> repository,
    IReservationCleanupQueue queue,
    IPushNotificationsService pushNotificationsService,
    ITenantSettingsCacheService tenantSettingsCacheService) : IRequestHandler<ApproveSpaceTableReservationCommand>
{
    readonly IRepository<SpaceTableReservation> repository = repository;
    readonly IReservationCleanupQueue queue = queue;
    readonly IPushNotificationsService pushNotificationsService = pushNotificationsService;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;

    public async Task Handle(ApproveSpaceTableReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await this.repository.GetByAsyncWithTracking(x => x.Id == request.ReservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(SpaceTableReservation), request.ReservationId);

        reservation.Status = ReservationStatus.Accepted;
        reservation.InternalNote = request.InternalNote;
        reservation.PublicNote = request.PublicNote;

        await this.repository.SaveChangesAsync(cancellationToken);

        var tenantSettings = await this.tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        await this.queue.UpdateReservationCleaningJob(
             reservation.Id, ReservationType.Table, reservation.ReservationDate.AddMinutes(tenantSettings.BonusTimeAfterReservationExpiration));

        var payload = new SpaceTableApprovedNotification
        {
            NumberOfGuests = reservation.NumberOfGuests,
            ReservationDate = reservation.ReservationDate
        };

        await this.pushNotificationsService
            .SendNotificationToUsersAsync([reservation.UserId], payload, cancellationToken);
    }
}
