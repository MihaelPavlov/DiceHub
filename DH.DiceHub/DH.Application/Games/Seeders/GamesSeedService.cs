using DH.Application.Games.Commands.Games;
using DH.Domain.Adapters.Data;
using DH.Domain.Services.Seed;
using MediatR;

namespace DH.Application.Games.Seeders;

internal class GamesSeedService : ISeedService
{
    readonly IMediator mediator;

    public GamesSeedService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task Seed()
    {
        foreach (var game in SeedData.GAME_LIST_DTOS)
        {
            var memoryStream = new MemoryStream();

            await this.mediator.Send(new CreateGameCommand(game, "game_image.png", "image/png", memoryStream));
        }
    }
}
