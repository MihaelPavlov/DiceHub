﻿namespace DH.Domain.Models.RewardModels.Queries;

public class GetSystemRewardListQueryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CashEquivalent { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ImageId { get; set; }
}
