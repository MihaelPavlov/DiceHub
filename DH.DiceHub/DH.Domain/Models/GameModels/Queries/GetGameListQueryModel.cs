﻿namespace DH.Domain.Models.GameModels.Queries;

public class GetGameListQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Likes { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
