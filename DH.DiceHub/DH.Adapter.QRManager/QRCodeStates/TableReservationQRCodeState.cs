using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;

namespace DH.Adapter.QRManager.QRCodeStates;

public class TableReservationQRCodeState(IUserContext userContext, IRepository<SpaceTableReservation> spaceTableReservationRepository) : IQRCodeState
{
    readonly IUserContext userContext = userContext;
    readonly IRepository<SpaceTableReservation> spaceTableReservationRepository = spaceTableReservationRepository;

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.TableReservation);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, "Access denied: only authorized staff can scan reservations.", traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, "Invalid request: User ID is missing.", traceId, cancellationToken);

        var tableReservation = await this.spaceTableReservationRepository
            .GetByAsyncWithTracking(x => x.Id == data.Id && x.UserId == userId && x.IsActive, cancellationToken);

        if (tableReservation == null)
            return await SetError(context, data, result, "No such active table reservation found for the specified user.", traceId, cancellationToken);

        if (tableReservation.Status != ReservationStatus.Accepted)
            return await SetError(context, data, result, "This table reservation is not approved from staff", traceId, cancellationToken);

        tableReservation.IsReservationSuccessful = true;
        tableReservation.IsActive = false;

        await this.spaceTableReservationRepository.SaveChangesAsync(cancellationToken);

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
