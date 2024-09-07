using DH.Domain.Services;
using MediatR;

namespace DH.Application.Rooms.Commands;

public record DeleteRoomCommand(int Id) : IRequest;

internal class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    readonly IRoomService roomService;

    public DeleteRoomCommandHandler(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        await this.roomService.Delete(request.Id, cancellationToken);
    }
}
