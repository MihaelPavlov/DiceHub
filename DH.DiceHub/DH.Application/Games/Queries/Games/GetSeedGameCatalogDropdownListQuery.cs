using DH.Domain.Adapters.FileManager;
using DH.Domain.Entities;
using DH.Domain.Models.GameModels.Queries;
using DH.Domain.Repositories;
using MediatR;

namespace DH.Application.Games.Queries.Games;

public record GetSeedGameCatalogDropdownListQuery : IRequest<List<GetSeedGameCatalogDropdownListQueryModel>>;

internal class GetSeedGameCatalogDropdownListQueryHandler(
    IRepository<SeedGameCatalog> repository,
    IFileManagerClient fileManagerClient) : IRequestHandler<GetSeedGameCatalogDropdownListQuery, List<GetSeedGameCatalogDropdownListQueryModel>>
{
    readonly IRepository<SeedGameCatalog> repository = repository;
    readonly IFileManagerClient fileManagerClient = fileManagerClient;

    public async Task<List<GetSeedGameCatalogDropdownListQueryModel>> Handle(
        GetSeedGameCatalogDropdownListQuery request,
        CancellationToken cancellationToken)
    {
        var seedGames = await this.repository.GetWithPropertiesAsync(
            x => x.IsActive,
            x => new GetSeedGameCatalogDropdownListQueryModel(
                x.Id,
                x.Name,
                x.CategoryName,
                x.MinPlayers,
                x.MaxPlayers,
                x.MinAge,
                x.AveragePlaytime,
                x.ImageUrl,
                x.ImageFileName),
            cancellationToken);

        return seedGames
            .Select(x => x with
            {
                ImageUrl = ResolveSeedImageUrl(x.ImageUrl, x.ImageFileName),
            })
            .ToList();
    }

    string ResolveSeedImageUrl(string imageUrl, string imageFileName)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl))
            return imageUrl;

        if (string.IsNullOrWhiteSpace(imageFileName))
            return string.Empty;

        return this.fileManagerClient.GetPublicUrl(FileManagerFolders.Seed.ToString(), imageFileName);
    }
}
