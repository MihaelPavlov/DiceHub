using DH.Domain.Enums;

namespace DH.Domain.Models.GameModels.Queries;

public record GetSeedGameCatalogDropdownListQueryModel(
    int Id,
    string Name,
    string CategoryName,
    int MinPlayers,
    int MaxPlayers,
    int MinAge,
    GameAveragePlaytime AveragePlaytime,
    string ImageUrl,
    string ImageFileName);
