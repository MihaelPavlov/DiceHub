using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.SpaceManagementModels.Commands;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record UpdateSpaceTableCommand(UpdateSpaceTableDto SpaceTable) : IRequest;

internal class UpdateSpaceTableCommandHandler(
    IRepository<SpaceTable> repository,
    IUserContext userContext,
    ILocalizationService localizer) : IRequestHandler<UpdateSpaceTableCommand>
{
    readonly IRepository<SpaceTable> repository = repository;
    readonly IUserContext userContext = userContext;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(UpdateSpaceTableCommand request, CancellationToken cancellationToken)
    {
        if (!request.SpaceTable.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var spaceTable = await this.repository.GetByAsyncWithTracking(x => x.Id == request.SpaceTable.Id, cancellationToken)
             ?? throw new NotFoundException(nameof(SpaceTable), request.SpaceTable.Id);

        if (spaceTable.CreatedBy != this.userContext.UserId)
            throw new BadRequestException("Only creator of the room can modify it");

        spaceTable.Name = request.SpaceTable.Name;
        spaceTable.MaxPeople = request.SpaceTable.MaxPeople;
        spaceTable.Password = request.SpaceTable.Password;
        spaceTable.IsLocked = string.IsNullOrEmpty(request.SpaceTable.Password) ? false : true;

        await this.repository.SaveChangesAsync(cancellationToken);
    }
}