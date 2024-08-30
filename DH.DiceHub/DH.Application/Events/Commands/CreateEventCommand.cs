using DH.Domain.Models.EventModels.Command;
using MediatR;

namespace DH.Application.Events.Commands;

public record CreateEventCommand(CreateEventModel Game, string FileName, string ContentType, MemoryStream ImageStream): IRequest
{
}
