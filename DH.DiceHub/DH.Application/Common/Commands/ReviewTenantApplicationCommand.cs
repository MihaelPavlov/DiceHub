using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record ReviewTenantApplicationCommand(int Id, TenantApplicationStatus Status, string? Note) : IRequest;

internal class ReviewTenantApplicationCommandHandler(
    IRepository<TenantApplication> repository,
    ISystemUserContextAccessor systemUserContextAccessor,
    IUserContext userContext,
    IMediator mediator) : IRequestHandler<ReviewTenantApplicationCommand>
{
    readonly IRepository<TenantApplication> repository = repository;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;
    readonly IUserContext userContext = userContext;
    readonly IMediator mediator = mediator;

    public async Task Handle(ReviewTenantApplicationCommand request, CancellationToken cancellationToken)
    {
        if (request.Status is not (TenantApplicationStatus.Verified or TenantApplicationStatus.Rejected))
            throw new BadRequestException("Tenant application can only be verified or rejected.");

        var application = await repository.GetByAsyncWithTracking(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TenantApplication), request.Id);

        if (application.Status != TenantApplicationStatus.PendingVerification)
            throw new BadRequestException("Tenant application has already been reviewed.");

        application.Status = request.Status;
        application.ReviewNote = request.Note;
        application.ReviewedByUserId = userContext.UserId;
        application.ReviewedDate = DateTime.UtcNow;

        systemUserContextAccessor.Set(new TenantApplicationSystemUserContext(userContext.UserId));
        await repository.SaveChangesAsync(cancellationToken);

        if (request.Status == TenantApplicationStatus.Verified)
        {
            await this.mediator.Send(new SendTenantSetupInvitationCommand(application.Id), cancellationToken);
        }
    }

    private sealed class TenantApplicationSystemUserContext(string? userId) : IUserContext
    {
        public string? TenantId => null;
        public string? UserId => userId ?? "tenant-application-review";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => true;
        public bool IsSystem => true;
    }
}
