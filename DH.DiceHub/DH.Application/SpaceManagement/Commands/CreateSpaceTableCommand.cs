using DH.Domain.Adapters.Authentication;
using DH.Domain.Entities;
using DH.Domain.Exceptions;
using DH.Domain.Models.SpaceManagementModels.Commands;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.SpaceManagement.Commands;

public record CreateSpaceTableCommand(CreateSpaceTableDto SpaceTable) : IRequest<int>;

internal class CreateSpaceTableCommandHandler : IRequestHandler<CreateSpaceTableCommand, int>
{
    readonly IRepository<SpaceTable> repository;
    readonly IUserContext userContext;

    public CreateSpaceTableCommandHandler(IRepository<SpaceTable> repository, IUserContext userContext)
    {
        this.repository = repository;
        this.userContext = userContext;
    }

    public async Task<int> Handle(CreateSpaceTableCommand request, CancellationToken cancellationToken)
    {
        if (!request.SpaceTable.FieldsAreValid(out var validationErrors))
            throw new ValidationErrorsException(validationErrors);

        var spaceTable = await this.repository.AddAsync(new SpaceTable
        {
            CreatedDate = DateTime.UtcNow,
            CreatedBy = this.userContext.UserId,
            GameId = request.SpaceTable.GameId,
            Password = request.SpaceTable.Password,
            IsLocked = !string.IsNullOrEmpty(request.SpaceTable.Password),
            MaxPeople = request.SpaceTable.MaxPeople,
            Name = request.SpaceTable.Name,
        }, cancellationToken);

        return spaceTable.Id;
    }
}