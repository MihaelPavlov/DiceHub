using DH.Adapter.Authentication.Filters;
using DH.Application.Common.Commands;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    readonly IPushNotificationsService pushNotificationsService;
    readonly IMediator mediator;
    public MessageController(IPushNotificationsService pushNotificationsService, IMediator mediator)
    {
        this.pushNotificationsService = pushNotificationsService;
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest request)
    {
        await this.pushNotificationsService.SendMessageAsync(request);
        return Ok("Push notification sent successfully!");
    }

    [HttpPost("save-token")]
    [ActionAuthorize(UserAction.MessagingCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveToken([FromBody] SaveUserDeviceTokenCommand command, CancellationToken cancellationToken)
    {
        await this.mediator.Send(command, cancellationToken);
        return Ok();
    }
}
