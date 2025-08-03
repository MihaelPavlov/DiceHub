using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Reservations;
using DH.Domain.Adapters.Statistics;
using DH.Domain.Adapters.Statistics.Services;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameReservationQRCodeState : IQRCodeState
{
    readonly IUserContext userContext;
    readonly IRepository<GameReservation> gameReservationRepository;
    readonly IRepository<SpaceTableReservation> tableReservationRepository;
    readonly IRepository<SpaceTable> tableRepository;
    readonly IRepository<Game> gameRepository;
    readonly IUserService userService;
    readonly ISpaceTableService spaceTableService;
    readonly SynchronizeGameSessionQueue gameSessionQueue;
    readonly IStatisticQueuePublisher statisticQueuePublisher;
    readonly ReservationCleanupQueue reservationCleanupQueue;

    public GameReservationQRCodeState(IUserContext userContext, IRepository<GameReservation> gameReservationRepository,
        IRepository<SpaceTableReservation> tableReservationRepository, IUserService userService,
        ISpaceTableService spaceTableService, IRepository<SpaceTable> tableRepository, SynchronizeGameSessionQueue gameSessionQueue,
        IRepository<Game> gameRepository, IStatisticQueuePublisher statisticQueuePublisher,
        ReservationCleanupQueue reservationCleanupQueue)
    {
        this.userContext = userContext;
        this.gameReservationRepository = gameReservationRepository;
        this.tableReservationRepository = tableReservationRepository;
        this.tableRepository = tableRepository;
        this.userService = userService;
        this.spaceTableService = spaceTableService;
        this.gameSessionQueue = gameSessionQueue;
        this.gameRepository = gameRepository;
        this.statisticQueuePublisher = statisticQueuePublisher;
        this.reservationCleanupQueue = reservationCleanupQueue;
    }

    public async Task<QrCodeValidationResult> HandleAsync(IQRCodeContext context, QRReaderModel data, CancellationToken cancellationToken)
    {
        var traceId = Guid.NewGuid().ToString();

        var result = new QrCodeValidationResult(data.Id, QrCodeType.GameReservation);

        if (this.userContext.RoleKey == (int)Role.User)
            return await SetError(context, data, result, "Access denied: only authorized staff can scan reservations.", traceId, cancellationToken);

        if (!data.AdditionalData.TryGetValue("userId", out var userId))
            return await SetError(context, data, result, "Invalid request: User ID is missing.", traceId, cancellationToken);

        var gameReservation = await this.gameReservationRepository
            .GetByAsyncWithTracking(x => x.Id == data.Id && x.UserId == userId, cancellationToken);

        if (gameReservation == null)
            return await SetError(context, data, result, "No such active game reservation found for the specified user.", traceId, cancellationToken);

        if (!gameReservation.IsActive)
            return await SetError(context, data, result, "This game reservation is no longer active", traceId, cancellationToken);

        if (gameReservation.Status != ReservationStatus.Accepted)
            return await SetError(context, data, result, "This game reservation is not approved from staff", traceId, cancellationToken);

        var activeTable = await this.tableRepository
            .GetByAsync(x => x.IsTableActive && x.CreatedBy == userId,
            cancellationToken);

        if (activeTable != null)
        {
            var message = string.Empty;

            if (activeTable.GameId == gameReservation.GameId)
                message = "User already have an active table for this game";
            else
                message = "User already have an active table for different game";

            return await SetError(context, data, result, message, traceId, cancellationToken);
        }

        gameReservation.IsReservationSuccessful = true;
        gameReservation.IsActive = false;

        var tableReservation = await this.tableReservationRepository
            .GetByAsyncWithTracking(x =>
                x.IsActive && x.Status == ReservationStatus.Accepted &&
                x.UserId == userId &&
                x.ReservationDate.Date == gameReservation.ReservationDate.Date,
            cancellationToken);

        if (tableReservation != null)
        {
            tableReservation.IsReservationSuccessful = true;
            tableReservation.IsActive = false;
            result.InternalNote = string.IsNullOrEmpty(tableReservation.InternalNote) ? null : tableReservation.InternalNote;
        }

        string? gameNote = string.IsNullOrEmpty(gameReservation.InternalNote) ? null : gameReservation.InternalNote;
        if (gameNote != null)
        {
            if (!string.IsNullOrEmpty(result.InternalNote))
                result.InternalNote += "; \n\n" + gameNote;
            else
                result.InternalNote = gameNote;
        }

        await this.gameReservationRepository.SaveChangesAsync(cancellationToken);

        var users = await this.userService.GetUserListByIds([userId], cancellationToken);
        var gameReservationUser = users.First();

        var request = new SpaceTable
        {
            GameId = gameReservation.GameId,
            IsSoloModeActive = gameReservation.NumberOfGuests == 1 ? true : false,
            MaxPeople = gameReservation.NumberOfGuests,
            Name = gameReservationUser.UserName,
            Password = string.Empty,
        };
        var spaceTableId = await this.spaceTableService.Create(request, cancellationToken, fromGameReservation: true, gameReservation.UserId);

        var game = await this.gameRepository.GetByAsync(x => x.Id == gameReservation.GameId, cancellationToken);
        if (game == null)
            return await SetError(context, data, result, $"{traceId}: Table id-{spaceTableId}, was created. But AddUserPlayTimEnforcerJob was not added to the queue, because game with id-{request.GameId} was not founded", traceId, cancellationToken);

        this.gameSessionQueue.AddUserPlayTimEnforcerJob(userId, game.Id, DateTime.UtcNow.AddMinutes((int)game.AveragePlaytime));

        await context.TrackScannedQrCode(traceId, data, null, cancellationToken);

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ClubActivityDetectedJob(
            userId, DateTime.UtcNow));

        await this.statisticQueuePublisher.PublishAsync(new StatisticJobQueue.ReservationProcessingOutcomeJob(
           userId, ReservationOutcome.Completed, ReservationType.Game, gameReservation.Id, DateTime.UtcNow));

        this.reservationCleanupQueue.RemoveReservationCleaningJob(gameReservation.Id);

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
