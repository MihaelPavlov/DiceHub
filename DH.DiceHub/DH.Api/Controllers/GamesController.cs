
using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Commands;
using DH.Application.Games.Commands.Games;
using DH.Application.Games.Queries;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Models.GameModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    readonly IMediator mediator;

    public GamesController(IMediator mediator )
    {
        this.mediator = mediator;
    }

    [HttpPost("list")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameList([FromBody] GetGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameDropdownListQueryModel>))]
    public async Task<IActionResult> GetDropdownList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameDropdownListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-new-games")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetNewGameList([FromBody] GetNewGameListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("get-games-by-category")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameListQueryModel>))]
    public async Task<IActionResult> GetGameListByCategoryId([FromBody] GetGameListByCategoryIdQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameByIdQueryModel))]
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

        var result = await this.mediator.Send(new CreateGameCommand(
            gameDto,
            imageFile.FileName,
            imageFile.ContentType,
            memoryStream
            ), cancellationToken);

        return Ok(result);
    }

    [HttpPost("copy")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateGameCopy([FromBody] CreateGameCopyCommand command, CancellationToken cancellationToken)
    {
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
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    [HttpPost("user-reservation-status")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameReservationStatusQueryModel))]
    public async Task<IActionResult> GetUserGameReservationStatus(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameReservationStatusQuery(null), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-active-reserved-game")]
    [ActionAuthorize(UserAction.GamesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActiveReservedGameQueryModel))]
    public async Task<IActionResult> GetActiveReservedGame(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetActiveReservedGameQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-active-reserved-games")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetActiveGameReservationListQueryModel>))]
    public async Task<IActionResult> GetActiveGameReservationList(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetActiveGameReservationListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-active-reserved-games-count")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IActionResult> GetActiveGameReservationCount(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetActiveGameReservationCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("approve-reservation")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveGameReservation([FromBody] ApproveGameReservationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut("decline-reservation")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeclineGameReservation([FromBody] DeclineGameReservationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPut("cancel-reservation")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelGameReservation([FromBody] CancelGameReservationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("get-reservation-history")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameReservationHistoryQueryModel>))]
    public async Task<IActionResult> GetReservationHistory([FromBody] GetGameReservationHistoryQuery query, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("get-reservation/{id}")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetGameReservationByIdQueryModel))]
    public async Task<IActionResult> GetReservationById(int id, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new GetGameReservationByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPut("update-reservation")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateReservation([FromBody] UpdateGameReservationCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("delete-reservation/{id}")]
    [ActionAuthorize(UserAction.GamesCUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteReservation(int id, CancellationToken cancellationToken)
    {
        await this.mediator.Send(new DeleteGameReservationCommand(id), cancellationToken);
        return Ok();
    }
}