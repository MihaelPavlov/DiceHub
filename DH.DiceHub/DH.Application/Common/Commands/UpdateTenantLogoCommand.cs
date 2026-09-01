using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Entities;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;

namespace DH.Application.Common.Commands;

public record UpdateTenantLogoCommand(string FileName, MemoryStream FileStream) : IRequest<string>;

internal class UpdateTenantLogoCommandHandler(
    IRepository<Tenant> tenantRepository,
    IUserContext userContext,
    IFileManagerClient fileManagerClient) : IRequestHandler<UpdateTenantLogoCommand, string>
{
    readonly IRepository<Tenant> tenantRepository = tenantRepository;
    readonly IUserContext userContext = userContext;
    readonly IFileManagerClient fileManagerClient = fileManagerClient;

    public async Task<string> Handle(UpdateTenantLogoCommand request, CancellationToken cancellationToken)
    {
        var tenantId = this.userContext.TenantId
            ?? throw new BadRequestException("Tenant context is required to update the club image.");

        var tenant = await this.tenantRepository.GetByAsyncWithTracking(x => x.Id == tenantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tenant), tenantId);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
        var logoUrl = await this.fileManagerClient.UploadFileAsync(
            FileManagerFolders.Tenants.ToString(), uniqueFileName, request.FileStream.ToArray());

        tenant.LogoFileName = logoUrl;
        await this.tenantRepository.SaveChangesAsync(cancellationToken);

        return logoUrl;
    }
}
