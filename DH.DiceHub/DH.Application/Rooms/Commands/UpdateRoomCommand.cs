using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.RoomModels.Commands;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Mapster;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record UpdateRoomCommand(UpdateRoomCommandDto Room) : IRequest;

internal class UpdateRoomCommandHanler : IRequestHandler<UpdateRoomCommand>
{
    readonly IRoomService roomService;
    readonly ILocalizationService localizer;

    public UpdateRoomCommandHanler(
        IRoomService roomService, ILocalizationService localizer)
    {
        this.roomService = roomService;
        this.localizer = localizer;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!request.Room.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        await this.roomService.Update(request.Room.Adapt<Room>(), cancellationToken);       
    }
}
