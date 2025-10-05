﻿using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Models.ChallengeModels.Queries;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class ChallengeService : IChallengeService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory;
    readonly IUserContext userContext;
    readonly ILocalizationService localizer;

    public ChallengeService(IDbContextFactory<TenantDbContext> _contextFactory, IUserContext userContext, ILocalizationService localizer)
    {
        this._contextFactory = _contextFactory;
        this.userContext = userContext;
        this.localizer = localizer;
    }

    public async Task<int> Create(Challenge challenge, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.ChallengeStatistics.AddAsync(new ChallengeStatistic
            {
                Challenge = challenge,
                TotalCompletions = 0
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return challenge.Id;
        }
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var challenge = await context.Challenges
                .Include(x => x.UserChallenges)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (challenge == null)
                throw new NotFoundException(nameof(Challenge), id);

            if (challenge.UserChallenges.Count != 0)
                throw new ValidationErrorsException("UserChallenges", this.localizer["DeleteChallengeDependenciesError"]);

            context.Challenges.Remove(challenge);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
    public async Task<List<GetUserUniversalChallengeListQueryModel>> GetUserUniversalChallenges(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var activeChallenges = await context.UserChallenges
                .Where(x => x.UserId == this.userContext.UserId
                         && x.IsActive
                         && x.UniversalChallenge != null)
                .Select(x =>
                    new GetUserUniversalChallengeListQueryModel
                    {
                        Id = x.Id,
                        RewardPoints = x.UniversalChallenge!.RewardPoints,
                        Status = x.Status,
                        CurrentAttempts = x.AttemptCount,
                        MaxAttempts = x.UniversalChallenge.Attempts,
                        GameImageId = x.Game != null ? x.Game.Image.Id : null,
                        GameName = x.Game != null ? x.Game.Name : null,
                        Type = x.UniversalChallenge.Type,
                        MinValue = x.UniversalChallenge.MinValue,
                        Name_EN = x.UniversalChallenge.Name_EN,
                        Name_BG = x.UniversalChallenge.Name_BG,
                        Description_EN = x.UniversalChallenge.Description_EN,
                        Description_BG = x.UniversalChallenge.Description_BG
                    })
                .ToListAsync(cancellationToken);

            var lastCompletedChallenge = await context.UserChallenges
                .Where(x => x.UserId == this.userContext.UserId
                         && x.CompletedDate != null
                         && x.UniversalChallenge != null)
                .OrderByDescending(x => x.CompletedDate)
                .Select(x =>
                    new GetUserUniversalChallengeListQueryModel
                    {
                        Id = x.Id,
                        RewardPoints = x.UniversalChallenge!.RewardPoints,
                        Status = x.Status,
                        CurrentAttempts = x.AttemptCount,
                        MaxAttempts = x.UniversalChallenge.Attempts,
                        GameImageId = x.Game != null ? x.Game.Image.Id : null,
                        GameName = x.Game != null ? x.Game.Name : null,
                        Type = x.UniversalChallenge.Type,
                        MinValue = x.UniversalChallenge.MinValue,
                        Name_EN = x.UniversalChallenge.Name_EN,
                        Name_BG = x.UniversalChallenge.Name_BG,
                        Description_EN = x.UniversalChallenge.Description_EN,
                        Description_BG = x.UniversalChallenge.Description_BG
                    })
                .FirstOrDefaultAsync(cancellationToken);

            var orderedChallenges = activeChallenges
                .OrderBy(x => x.Status)
                .ThenBy(x => x.RewardPoints)
                .ToList();

            if (lastCompletedChallenge != null)
                orderedChallenges.Add(lastCompletedChallenge);

            return orderedChallenges;
        }
    }

    public async Task<List<GetUserChallengeListQueryModel>> GetUserChallenges(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var activeChallenges = await context.UserChallenges
                .Where(x => this.userContext.UserId == x.UserId && x.IsActive && x.Challenge != null)
                .Select(x =>
                    new GetUserChallengeListQueryModel
                    {
                        Id = x.Id,
                        RewardPoints = x.Challenge!.RewardPoints,
                        Status = x.Status,
                        GameImageId = x.Challenge.Game.Image.Id,
                        GameName = x.Challenge.Game.Name,
                        CurrentAttempts = x.AttemptCount,
                        MaxAttempts = x.Challenge.Attempts,
                    })
                .ToListAsync(cancellationToken);

            var lastCompletedChallenge = await context.UserChallenges
                .Where(x => this.userContext.UserId == x.UserId && x.CompletedDate != null && x.Challenge != null)
                .OrderByDescending(x => x.CompletedDate)
                .Select(x =>
                    new GetUserChallengeListQueryModel
                    {
                        Id = x.Id,
                        RewardPoints = x.Challenge!.RewardPoints,
                        Status = x.Status,
                        GameImageId = x.Challenge.Game.Image.Id,
                        GameName = x.Challenge.Game.Name,
                        CurrentAttempts = x.AttemptCount,
                        MaxAttempts = x.Challenge.Attempts,
                    })
                .FirstOrDefaultAsync(cancellationToken);

            var orderedChallenges = activeChallenges
                .OrderBy(x => x.Status)
                .ThenBy(x => x.RewardPoints)
                .ToList();

            if (lastCompletedChallenge != null)
                orderedChallenges.Add(lastCompletedChallenge);

            return orderedChallenges;
        }
    }

    public async Task SaveCustomPeriod(SaveCustomPeriodDto customPeriod, CancellationToken cancellationToken)
    {
        using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var rewardIds = customPeriod.Rewards.Select(r => r.SelectedReward).ToHashSet();
        var gameIds = customPeriod.Challenges.Select(c => c.SelectedGame).ToHashSet();

        // Validate rewards
        var existingRewardRefs = await context.ChallengeRewards
            .Where(x => rewardIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (existingRewardRefs.Count != rewardIds.Count)
            throw new ValidationErrorsException("Rewards", this.localizer["SaveCustomPeriodRewardsError"]);

        // Validate games
        var existingGameRefs = await context.Games
            .Where(x => gameIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (existingGameRefs.Count != gameIds.Count)
            throw new ValidationErrorsException("Challenges", this.localizer["SaveCustomPeriodChallengesError"]);

        // --- Rewards ---
        var incomingRewardDtos = customPeriod.Rewards;
        var incomingRewardIds = incomingRewardDtos.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToHashSet();

        var existingRewards = await context.CustomPeriodRewards
            .Where(r => incomingRewardIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        foreach (var dto in incomingRewardDtos)
        {
            if (dto.Id is null)
            {
                await context.CustomPeriodRewards.AddAsync(new CustomPeriodReward
                {
                    RewardId = dto.SelectedReward,
                    RequiredPoints = dto.RequiredPoints,
                }, cancellationToken);
            }
            else if (existingRewards.FirstOrDefault(r => r.Id == dto.Id) is { } entity)
            {
                entity.RewardId = dto.SelectedReward;
                entity.RequiredPoints = dto.RequiredPoints;
            }
        }

        // --- Challenges ---
        var incomingChallengeDtos = customPeriod.Challenges;
        var incomingChallengeIds = incomingChallengeDtos.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToHashSet();

        var existingChallenges = await context.CustomPeriodChallenges
            .Where(c => incomingChallengeIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var dto in incomingChallengeDtos)
        {
            if (dto.Id is null)
            {
                await context.CustomPeriodChallenges.AddAsync(new CustomPeriodChallenge
                {
                    GameId = dto.SelectedGame,
                    Attempts = dto.Attempts,
                    RewardPoints = dto.Points,
                }, cancellationToken);
            }
            else if (existingChallenges.FirstOrDefault(c => c.Id == dto.Id) is { } entity)
            {
                entity.GameId = dto.SelectedGame;
                entity.Attempts = dto.Attempts;
                entity.RewardPoints = dto.Points;
            }
        }

        // --- Universal Challenges ---
        var incomingUniversalDtos = customPeriod.UniversalChallenges;
        var incomingUniversalIds = incomingUniversalDtos.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToHashSet();

        var existingUniversalChallenges = await context.CustomPeriodUniversalChallenges
            .Where(c => incomingUniversalIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var dto in incomingUniversalDtos)
        {
            if (dto.Id is null)
            {
                await context.CustomPeriodUniversalChallenges.AddAsync(new CustomPeriodUniversalChallenge
                {
                    UniversalChallengeId = dto.SelectedUniversalChallenge,
                    Attempts = dto.Attempts,
                    RewardPoints = dto.Points,
                    MinValue = dto.MinValue
                }, cancellationToken);
            }
            else if (existingUniversalChallenges.FirstOrDefault(c => c.Id == dto.Id) is { } entity)
            {
                entity.UniversalChallengeId = dto.SelectedUniversalChallenge;
                entity.Attempts = dto.Attempts;
                entity.RewardPoints = dto.Points;
                entity.MinValue = dto.MinValue;
            }
        }

        // --- Soft Delete Missing Rewards ---
        var allRewards = await context.CustomPeriodRewards.ToListAsync(cancellationToken);
        var rewardForDelete = allRewards.Where(r => !incomingRewardIds.Contains(r.Id));
        context.RemoveRange(rewardForDelete);

        // --- Soft Delete Missing Challenges ---
        var allChallenges = await context.CustomPeriodChallenges.ToListAsync(cancellationToken);
        var challengeForDelete = allChallenges.Where(c => !incomingChallengeIds.Contains(c.Id));
        context.RemoveRange(challengeForDelete);

        // --- Soft Delete Missing Universal Challenges ---
        var allUniversalChallenges = await context.CustomPeriodUniversalChallenges.ToListAsync(cancellationToken);
        var universalForDelete = allUniversalChallenges.Where(c => !incomingUniversalIds.Contains(c.Id));
        context.RemoveRange(universalForDelete);

        if (customPeriod.Rewards.Count != 0 && customPeriod.Challenges.Count != 0)
        {
            var settings = await context.TenantSettings.AsTracking().FirstAsync(x => x.Id == 1, cancellationToken);
            settings.IsCustomPeriodSetupComplete = true;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<GetUserCustomPeriodQueryModel> GetUserCustomPeriodData(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var period = await context.UserChallengePeriodPerformances
                .Include(x => x.CustomPeriodUserChallenges)
                    .ThenInclude(x => x.Game)
                        .ThenInclude(x => x.Image)
                .Include(x => x.CustomPeriodUserRewards)
                    .ThenInclude(x => x.Reward)
                        .ThenInclude(x => x.Image)
                .Include(x => x.CustomPeriodUserUniversalChallenges)
                    .ThenInclude(x => x.Game)
                        .ThenInclude(x => x.Image)
                .Include(x => x.CustomPeriodUserUniversalChallenges)
                    .ThenInclude(x => x.UniversalChallenge)

                .FirstOrDefaultAsync(x => x.UserId == this.userContext.UserId && x.IsPeriodActive, cancellationToken);

            if (period == null)
                throw new NotFoundException(nameof(period), this.userContext.UserId);

            var challenges = period.CustomPeriodUserChallenges.Select(x => new GetUserCustomPeriodChallengeQueryModel
            {
                ChallengeAttempts = x.ChallengeAttempts,
                CurrentAttempts = x.UserAttempts,
                GameImageId = x.Game.Image.Id,
                GameName = x.Game.Name,
                IsCompleted = x.IsCompleted,
                RewardPoints = x.RewardPoints,
            }).ToList();

            var rewards = period.CustomPeriodUserRewards.OrderBy(x => x.RequiredPoints).Select(x => new GetUserCustomPeriodRewardQueryModel
            {
                IsCompleted = x.IsCompleted,
                RewardRequiredPoints = x.RequiredPoints,
                RewardImageId = x.Reward.Image.Id,
            }).ToList();

            var universalChallenges = period.CustomPeriodUserUniversalChallenges.OrderBy(x => x.UniversalChallengeId).Select(x => new GetUserCustomPeriodUniversalChallengeQueryModel
            {
                MaxAttempts = x.ChallengeAttempts,
                CurrentAttempts = x.UserAttempts,
                GameImageId = x.Game != null ? x.Game.Image.Id : null,
                GameName = x.Game != null ? x.Game.Name : null,
                RewardPoints = x.RewardPoints,
                Type = x.UniversalChallenge.Type,
                Description_EN = x.UniversalChallenge.Description_EN,
                Description_BG = x.UniversalChallenge.Description_BG,
                Name_BG = x.UniversalChallenge.Name_BG,
                Name_EN = x.UniversalChallenge.Name_EN,
                MinValue = x.MinValue,
                Status = x.IsCompleted ? Domain.Enums.ChallengeStatus.Completed : Domain.Enums.ChallengeStatus.InProgress
            }).ToList();

            return new GetUserCustomPeriodQueryModel
            {
                Rewards = rewards,
                Challenges = challenges,
                UniversalChallenges = universalChallenges
            };
        }
    }
}
