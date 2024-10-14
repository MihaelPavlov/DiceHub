using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.SpaceManagementModels.Commands;
using DH.Domain.Services;
using Mapster;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record CreateSpaceTableCommand(CreateSpaceTableDto SpaceTable) : IRequest<int>;

internal class CreateSpaceTableCommandHandler : IRequestHandler<CreateSpaceTableCommand, int>
{
    readonly ISpaceTableService spaceTableService;

    public CreateSpaceTableCommandHandler(ISpaceTableService spaceTableService)
    {
        this.spaceTableService = spaceTableService;
    }

    public async Task<int> Handle(CreateSpaceTableCommand request, CancellationToken cancellationToken)
    {
        if (!request.SpaceTable.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var spaceTableId = await this.spaceTableService.Create(request.SpaceTable.Adapt<SpaceTable>(), cancellationToken);

        return spaceTableId;
    }
}