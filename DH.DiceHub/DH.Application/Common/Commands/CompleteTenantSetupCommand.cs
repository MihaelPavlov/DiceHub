using System.Security.Cryptography;
using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Scheduling;
using DH.Domain.Models.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DH.Application.Common.Commands;

public record CompleteTenantSetupCommand(CompleteTenantSetupRequest Request) : IRequest<CompleteTenantSetupResult>;

internal class CompleteTenantSetupCommandHandler(
    ITenantSetupService tenantSetupService,
    IOwnerService ownerService,
    ISchedulerService schedulerService,
    ILogger<CompleteTenantSetupCommandHandler> logger,
    IMediator mediator) : IRequestHandler<CompleteTenantSetupCommand, CompleteTenantSetupResult>
{
    readonly ITenantSetupService tenantSetupService = tenantSetupService;
    readonly IOwnerService ownerService = ownerService;
    readonly ISchedulerService schedulerService = schedulerService;
    readonly ILogger<CompleteTenantSetupCommandHandler> logger = logger;
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

        // Register this tenant's per-tenant daily jobs now, at creation - they fire
        // at tenant-local times (closing time, 06:00, 23:30) so each needs its own
        // trigger in the tenant's zone. A failure here must not fail tenant setup;
        // the startup reconciler back-fills any missing trigger.
        try
        {
            await this.schedulerService.ScheduleTenantDailyJobsAsync(
                setupResult.TenantId,
                request.Request.EndWorkingHours,
                request.Request.TimeZoneId,
                cancellationToken);

            await this.schedulerService.ScheduleAddUserPeriodJobForTenant(
                setupResult.TenantId, replaceExisting: false, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex,
                "Failed to schedule per-tenant daily jobs for newly created tenant {TenantId}.",
                setupResult.TenantId);
        }

        return setupResult;
    }

    static string HashToken(string token)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
