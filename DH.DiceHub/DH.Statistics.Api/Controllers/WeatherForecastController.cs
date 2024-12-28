using DH.Statistics.Application;
using DH.Statistics.Domain.Exceptions;
using DH.Statistics.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DH.Statistics.Domain.Exceptions.ValidationErrorsException;

namespace DH.Statistics.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    readonly IMediator mediator;

    public WeatherForecastController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        var errors = new List<ValidationError>();
        errors.Add(new ValidationError("Email", "Invalid Email"));
        errors.Add(new ValidationError("Email", "Email already exists"));
        errors.Add(new ValidationError("Phone", "Invalid Phone number"));
        throw new ValidationErrorsException(errors);
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OperationResult<int>))]
    public async Task<IActionResult> CreateLog([FromBody] CreateClubVisitorLogRequest request, CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new CreateClubVisitorLogCommand(request), cancellationToken);
        return Ok(result);
    }
}
