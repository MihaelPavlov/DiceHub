using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Commands;
using DH.Application.Games.Commands.Games;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Models.GameModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DH.Domain.Adapters.QRManager;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    readonly IMediator mediator;
    readonly IQRCodeManager qRCodeManager;
    readonly IWebHostEnvironment hostEnvironment;

    public GamesController(IMediator mediator, IQRCodeManager qRCodeManager, IWebHostEnvironment hostEnvironment)
    {
        this.mediator = mediator;
        this.qRCodeManager = qRCodeManager;
        this.hostEnvironment = hostEnvironment;
    }

    //[HttpPost("upload")]
    //[ActionAuthorize(UserAction.GamesRead)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    //public async Task<IActionResult> UploadQrCode(QrCodeRequest request, CancellationToken cancellationToken)
    //{
    //    await this.qRCodeManager.ProcessQRCodeAsync(request.Data, cancellationToken);
    //    return Ok(request);
    //}

    //[HttpPost("create-qr-code")]
    //[ActionAuthorize(UserAction.GamesRead)]
    //public IActionResult GenerateQRCode([FromBody] QrCodeRequest request)
    //{
    //    string webRootPath = _hostEnvironment.WebRootPath;

    //    this.qRCodeManager.CreateQRCode(request.QrCodeData, webRootPath);
    //    return Ok();
    //}

    [HttpPost("list")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameList(GetGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }


    [HttpGet("{id}/get-game-qr-code")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameQrCodeQueryModel))]
    public async Task<IActionResult> GetGameQrCode(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameQrCodeQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/inventory")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameInvetoryQueryModel))]
    public async Task<IActionResult> GetGameInventory(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameInventoryQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-reserved-games")]
    [ActionAuthorize(UserAction.GameReservedCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameReservationListQueryModel>))]
    public async Task<IActionResult> GetGameReservationList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameReservedListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-dropdown-list")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetDropdownList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameDropdownListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-new-games")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameDropdownListQueryModel>))]
    public async Task<IActionResult> GetNewGameList(GetNewGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-games-by-category")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameListByCategoryId(GetGameListByCategoryIdQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSystemRewardByIdQueryModel))]
    public async Task<IActionResult> GetGameById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> CreateGame([FromForm] string game, [FromForm] IFormFile imageFile, CancellationToken cancellationToken)
    {
        var gameDto = JsonSerializer.Deserialize<CreateGameDto>(game)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream, cancellationToken);
        string webRootPath = this.hostEnvironment.WebRootPath;

        var result = await this.mediator.Send(new CreateGameCommand(
            gameDto,
            imageFile.FileName,
            imageFile.ContentType,
            memoryStream,
            webRootPath
            ), cancellationToken);

        return Ok(result);
    }

    [HttpPost("copy")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGameCopy([FromBody] CreateGameCopyCommand command, CancellationToken cancellationToken)
    {
        string webRootPath = this.hostEnvironment.WebRootPath;
        command.webRootPath = webRootPath;

        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateGame([FromForm] string game, [FromForm] IFormFile? imageFile, CancellationToken cancellationToken)
    {
        var gameDto = JsonSerializer.Deserialize<UpdateGameDto>(game)
            ?? throw new JsonException();

        using var memoryStream = new MemoryStream();
        if (imageFile != null)
        {
            await imageFile.CopyToAsync(memoryStream, cancellationToken);
        }

        await this.mediator.Send(new UpdateGameCommand(gameDto, imageFile?.FileName, imageFile?.ContentType, memoryStream), cancellationToken);
        return Ok();
    }

    [HttpPut("{id}/like")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LikeGame(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new LikeGameCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPut("{id}/dislike")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DislikeGame(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DislikeGameCommand(id), cancellationToken);
        return Ok();
    }

    [HttpGet("get-image/{id}")]
    public async Task<IActionResult> GetGameImage(int id, CancellationToken cancellationToken)
    {
        var gameFile = await this.mediator.Send(new GetGameImageByIdQuery(id), cancellationToken);
        if (gameFile == null)
        {
            return NotFound();
        }

        return File(gameFile.Data, gameFile.ContentType, gameFile.FileName);
    }

    [HttpDelete("{id}")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteGame(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteGameCommand(id), cancellationToken);
        return Ok();
    }

    [HttpPost("reservation")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> CreateGameReservation([FromBody] CreateGameReservationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("reservation-status")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameReservationStatusQueryModel))]
    public async Task<IActionResult> GetGameReservationStatus([FromBody] GetGameReservationStatusQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
