using System.Security.Cryptography;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Models.Common;
using MediatR;

namespace DH.Application.Common.Commands;

public record CompleteTenantSetupCommand(CompleteTenantSetupRequest Request) : IRequest<CompleteTenantSetupResult>;

internal class CompleteTenantSetupCommandHandler(
    ITenantSetupService tenantSetupService,
    IOwnerService ownerService,
    IMediator mediator) : IRequestHandler<CompleteTenantSetupCommand, CompleteTenantSetupResult>
{
    readonly ITenantSetupService tenantSetupService = tenantSetupService;
    readonly IOwnerService ownerService = ownerService;
    readonly IMediator mediator = mediator;

    public async Task<CompleteTenantSetupResult> Handle(
        CompleteTenantSetupCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.Request.Token);
        var setupResult = await this.tenantSetupService.CompleteTenantSetupData(
            request.Request,
            tokenHash,
            cancellationToken);

        var ownerResult = await this.ownerService.CreateOwnerForTenantSetup(
            new CreateOwnerForTenantSetupRequest
            {
                TenantId = setupResult.TenantId,
                Email = setupResult.OwnerEmail,
                ClubPhoneNumber = request.Request.ClubPhoneNumber,
            },
            cancellationToken);

        await this.mediator.Send(
            new SendTenantOwnerCredentialsEmailCommand(
                setupResult.TenantId,
                setupResult.TenantName,
                ownerResult.Email,
                ownerResult.TemporaryPassword),
            cancellationToken);

        await this.tenantSetupService.MarkSetupTokenAsUsed(tokenHash, cancellationToken);

        return setupResult;
    }

    static string HashToken(string token)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
