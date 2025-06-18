using DH.OperationResultCore.FrontEndErrors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("frontend-log")]
public class FrontendLogController : ControllerBase
{
    private readonly ILogger<FrontendLogController> logger;

    public FrontendLogController(ILogger<FrontendLogController> logger)
    {
        this.logger = logger;
    }

    [HttpPost("error")]
    public IActionResult LogError([FromBody] ErrorBody request, CancellationToken cancellationToken)
    {
        logger.LogError("🔴 Frontend Error: {Message}\n📍 StackTrace: {Stack}", request.Message, request.Stack);
        return Ok();
    }

    [HttpPost("warning")]
    public IActionResult LogWarning([FromBody] ErrorBody request, CancellationToken cancellationToken)
    {
        logger.LogWarning("🟠 Frontend Warning: {Message}\n📍 StackTrace: {Stack}", request.Message, request.Stack);
        return Ok();
    }

    [HttpPost("info")]
    public IActionResult LogInfo([FromBody] ErrorBody request, CancellationToken cancellationToken)
    {
        logger.LogInformation("🟠 Frontend Info: {Message}\n📍 StackTrace: {Stack}", request.Message, request.Stack);
        return Ok();
    }
}

