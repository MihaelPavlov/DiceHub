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
[Route("api/[controller]")]
public class ScannerController : ControllerBase
{
    readonly IQRCodeManager qRCodeManager;
    readonly IQrTokenService qrTokenService;

    public ScannerController(IQRCodeManager qRCodeManager, IQrTokenService qrTokenService)
    {
        this.qRCodeManager = qRCodeManager;
        this.qrTokenService = qrTokenService;
    }

    [HttpPost("upload")]
    [ActionAuthorize(UserAction.ScannerRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QrCodeValidationResult))]
    public async Task<IActionResult> UploadQrCode([FromBody] QrCodeRequestModel request, CancellationToken cancellationToken)
    {
        var result = await this.qRCodeManager.ValidateQRCodeAsync(request.Data, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns the short opaque token to encode in a QR for a given entity, so the
    /// code stays small and scans reliably. Any signed-in user can issue their own
    /// reservation / reward codes; no ScannerRead needed.
    /// </summary>
    [HttpPost("issue")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QrIssueResponse))]
    public async Task<IActionResult> Issue([FromBody] QrIssueRequest request, CancellationToken cancellationToken)
    {
        var token = await this.qrTokenService.IssueAsync(request.Type, request.EntityId, cancellationToken);
        return Ok(new QrIssueResponse { Token = token });
    }
}
