using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Localization;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;

namespace DH.Adapter.QRManager.QRCodeStates;

public class TableReservationQRCodeState(
    IUserContext userContext, IRepository<SpaceTableReservation> spaceTableReservationRepository,
    IRepository<SpaceTable> spaceTableRepository, IStatisticQueuePublisher statisticQueuePublisher,
    ILocalizationService loc) : IQRCodeState
{
    readonly IUserContext userContext = userContext;
    readonly IRepository<SpaceTableReservation> spaceTableReservationRepository = spaceTableReservationRepository;
    readonly IRepository<SpaceTable> spaceTableRepository = spaceTableRepository;
    readonly IStatisticQueuePublisher statisticQueuePublisher = statisticQueuePublisher;
    readonly ILocalizationService loc = loc;

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.TableReservation);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, this.loc["TableReservationQrCodeScannedAccessDenied"], traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, this.loc["QrCodeScannedMissingUserId"], traceId, cancellationToken);

        var tableReservation = await this.spaceTableReservationRepository
            .GetByAsyncWithTracking(x => x.Id == data.Id && x.UserId == userId && x.IsActive, cancellationToken);

        if (tableReservation == null)
            return await SetError(context, data, result, this.loc["TableReservationQrCodeScannedNoActiveTableReservation"], traceId, cancellationToken);

        if (tableReservation.Status != ReservationStatus.Accepted)
            return await SetError(context, data, result, this.loc["TableReservationQrCodeScannedTableReservationNotApproved"], traceId, cancellationToken);

        var activeTable = await this.spaceTableRepository
           .GetByAsync(x => x.IsTableActive && x.CreatedBy == userId,
           cancellationToken);

        if (activeTable != null)
            return await SetError(context, data, result, this.loc["TableReservationQrCodeScannedUserAlreadyHasActiveTable"], traceId, cancellationToken);

        tableReservation.IsReservationSuccessful = true;
        tableReservation.IsActive = false;

        await this.spaceTableReservationRepository.SaveChangesAsync(cancellationToken);

        await context.TrackScannedQrCode(traceId, data, null, cancellationToken);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
            userId,
            ReservationOutcome.Completed,
            ReservationType.Table,
            tableReservation.Id,
            DateTime.UtcNow));

        result.InternalNote = string.IsNullOrEmpty(tableReservation.InternalNote) ? null : tableReservation.InternalNote;
        result.IsValid = true;
        return result;
    }

    private async Task<QrCodeValidationResult> SetError(IQRCodeContext context, QRReaderModel data, QrCodeValidationResult result, string errorMessage, string traceId, CancellationToken cancellationToken)
    {
        result.ErrorMessage = errorMessage;
        result.IsValid = false;
        await context.TrackScannedQrCode(traceId, data, new BadRequestException(errorMessage), cancellationToken);
        return result;
    }
}
