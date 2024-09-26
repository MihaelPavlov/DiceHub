using DH.Adapter.Authentication.Filters;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.QRManager;
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
    public async Task<IActionResult> UploadQrCode(UploadImage request, CancellationToken cancellationToken)
    {
        await this.qRCodeManager.ProcessQRCodeAsync(request.imageData, cancellationToken);
        return Ok();
    }
}
