using DH.Adapter.Authentication.Filters;
using DH.Application.Games.Queries.Games;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Models.GameModels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameCategoriesController : ControllerBase
{
    readonly IMediator mediator;

    public GameCategoriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("list")]
    [ActionAuthorize(UserAction.GameCategoriesCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetGameCategoryListQueryModel>))]
    public async Task<IActionResult> GetGameCategoryList(GetGameCategoryListQuery request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}
