using DH.Adapter.Authentication.Filters;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Models.ScannerModels.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ScannerController : ControllerBase
{
    readonly IQRCodeManager qRCodeManager;

    public ScannerController(IQRCodeManager qRCodeManager)
    {
        this.qRCodeManager = qRCodeManager;
    }

    [HttpPost("upload")]
    [ActionAuthorize(UserAction.ScannerRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadQrCode([FromBody] QrCodeRequestModel request, CancellationToken cancellationToken)
    {
        await this.qRCodeManager.ProcessQRCodeAsync(request.Data, cancellationToken);
        return Ok();
    }
}
