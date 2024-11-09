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
}
