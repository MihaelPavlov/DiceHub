using DH.Domain.Adapters.Data;
using DH.Domain.Cqrs;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Cqrs.GameCqrs;

public class CreateGameCommand : ICommand<int>
{
    public CreateGameDto Game { get; set; } = null!;
}

internal interface ICreateGameCommandHandler : IDefaultCommandHandler<CreateGameCommand, int> { }