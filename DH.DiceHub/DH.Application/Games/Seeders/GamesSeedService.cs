using DH.Application.Games.Commands.Games;
using DH.Domain.Adapters.Data;
using DH.Domain.Helpers;
using DH.Domain.Services.Seed;
using MediatR;

namespace DH.Application.Games.Seeders;

internal class GamesSeedService : ISeedService
{
    readonly IMediator mediator;
    readonly IWebRootPathHelper webRootPathHelper;

    public GamesSeedService(IMediator mediator, IWebRootPathHelper webRootPathHelper)
    {
        this.mediator = mediator;
        this.webRootPathHelper = webRootPathHelper;
    }

    public async void Seed()
    {
        foreach (var game in SeedData.GAME_LIST_DTOS)
        {
            var memoryStream = new MemoryStream();

            await this.mediator.Send(new CreateGameCommand(game, "game_image.png", "image/png", memoryStream, this.webRootPathHelper.GetWebRootPath));
        }
    }
}
