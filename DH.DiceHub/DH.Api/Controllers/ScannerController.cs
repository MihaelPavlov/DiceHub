using DH.Adapter.Authentication.Filters;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.QRManager;
using DH.Domain.Adapters.QRManager.StateModels;
using DH.Domain.Models.ScannerModels.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QrCodeValidationResult))]
    public async Task<IActionResult> UploadQrCode([FromBody] QrCodeRequestModel request, CancellationToken cancellationToken)
    {
        var result = await this.qRCodeManager.ValidateQRCodeAsync(request.Data, cancellationToken);
        return Ok(result);
    }
}
