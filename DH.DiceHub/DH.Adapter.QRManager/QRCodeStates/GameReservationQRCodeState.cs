using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.GameSession;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Exceptions;
using DH.Domain.Repositories;
using DH.Domain.Services;

namespace DH.Adapter.QRManager.QRCodeStates;

public class GameReservationQRCodeState : IQRCodeState
{
    readonly IUserContext userContext;
    readonly IRepository<GameReservation> gameReservationRepository;
    readonly IRepository<Game> gameRepository;
    readonly IUserService userService;
    readonly ISpaceTableService spaceTableService;
    readonly SynchronizeGameSessionQueue queue;
    readonly IJobManager jobManager;

    public GameReservationQRCodeState(IUserContext userContext, IRepository<GameReservation> gameReservationRepository, IUserService userService, ISpaceTableService spaceTableService, SynchronizeGameSessionQueue queue, IJobManager jobManager, IRepository<Game> gameRepository)
    {
        this.userContext = userContext;
        this.gameReservationRepository = gameReservationRepository;
        this.userService = userService;
        this.spaceTableService = spaceTableService;
        this.queue = queue;
        this.jobManager = jobManager;
        this.gameRepository = gameRepository;
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

        gameReservation.IsReservationSuccessful = true;
        gameReservation.IsActive = false;
        await this.gameReservationRepository.SaveChangesAsync(cancellationToken);

        var users = await this.userService.GetUserListByIds([userId], cancellationToken);
        var gameReservationUser = users.First();

        var request = new SpaceTable
        {
            GameId = gameReservation.GameId,
            IsSoloModeActive = gameReservation.PeopleCount == 1 ? true : false,
            MaxPeople = gameReservation.PeopleCount,
            Name = gameReservationUser.UserName,
            Password = string.Empty,
        };
        var spaceTableId = await this.spaceTableService.Create(request, cancellationToken, fromGameReservation: true, gameReservation.UserId);

        var game = await this.gameRepository.GetByAsync(x => x.Id == gameReservation.GameId, cancellationToken);
        if (game == null)
            return await SetError(context, data, result, $"{traceId}: Table id-{spaceTableId}, was created. But AddUserPlayTimEnforcerJob was not added to the queue, because game with id-{request.GameId} was not founded", traceId, cancellationToken);

        this.queue.AddUserPlayTimEnforcerJob(this.userContext.UserId, game.Id, DateTime.UtcNow.AddMinutes((int)game.AveragePlaytime));

        await context.TrackScannedQrCode(traceId, data, null, cancellationToken);
        await this.jobManager.DeleteJob($"ExpireReservationJob-{gameReservation.Id}", "ReservationJobs");

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
