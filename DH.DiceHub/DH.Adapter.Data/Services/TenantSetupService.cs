using System.Text.RegularExpressions;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.Common;
using DH.OperationResultCore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

public class TenantSetupService(
    TenantDbContext db,
    ISystemUserContextAccessor systemUserContextAccessor,
    IFileManagerClient fileManagerClient) : ITenantSetupService
{
    readonly TenantDbContext db = db;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;
    readonly IFileManagerClient fileManagerClient = fileManagerClient;

    public async Task<CompleteTenantSetupResult> CompleteTenantSetupData(
        CompleteTenantSetupRequest request,
        string tokenHash,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var setupToken = await this.db.TenantSetupTokens
            .AsTracking()
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UsedAt == null && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken)
            ?? throw new ValidationErrorsException("Token", "Setup link is invalid or expired.");

        var application = await this.db.TenantApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == setupToken.TenantApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(TenantApplication), setupToken.TenantApplicationId);

        if (application.Status != TenantApplicationStatus.Verified)
            throw new ValidationErrorsException("Application", "Tenant application is not verified.");

        if (!string.Equals(application.Email.Trim(), setupToken.Email, StringComparison.OrdinalIgnoreCase))
            throw new ValidationErrorsException("Email", "Setup email does not match the approved venue email.");

        var tenantId = await CreateUniqueTenantId(request.ClubName, cancellationToken);
        var selectedSeeds = await this.db.SeedGameCatalog
            .AsNoTracking()
            .Where(x => x.IsActive && request.SelectedGameIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (selectedSeeds.Count != request.SelectedGameIds.Distinct().Count())
            throw new ValidationErrorsException("SelectedGameIds", "One or more selected games are not available.");

        var settings = new TenantSetting
        {
            ClubName = request.ClubName.Trim(),
            AverageMaxCapacity = request.AverageMaxCapacity,
            PhoneNumber = request.ClubPhoneNumber.Trim(),
            DaysOff = string.Join(",", request.DaysOff.Select(x => x.Trim()).Where(x => x.Length > 0).Distinct()),
            StartWorkingHours = request.StartWorkingHours.Trim(),
            EndWorkingHours = request.EndWorkingHours.Trim(),
            ChallengeRewardsCountForPeriod = 5,
            PeriodOfRewardReset = "Weekly",
            ResetDayForRewards = "Sunday",
            ChallengeInitiationDelayHours = 6,
            ReservationHours = $"{request.StartWorkingHours.Trim()}, {request.EndWorkingHours.Trim()}",
            BonusTimeAfterReservationExpiration = 10,
            IsCustomPeriodOn = false,
            IsCustomPeriodSetupComplete = false,
        };

        await this.db.Tenants.AddAsync(new Tenant
        {
            Id = tenantId,
            TenantName = request.ClubName.Trim(),
            Town = application.Address,
            TenantStatus = TenantStatus.Active,
            CreatedDate = DateTime.UtcNow,
            LogoFileName = string.Empty,
            RegisterQrCode = string.Empty,
            TenantSetting = settings,
        }, cancellationToken);

        this.systemUserContextAccessor.Set(new TenantSetupSystemUserContext(tenantId));
        await CreateSeededGames(tenantId, selectedSeeds, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);

        return new CompleteTenantSetupResult
        {
            TenantId = tenantId,
            TenantName = request.ClubName.Trim(),
            OwnerEmail = setupToken.Email,
        };
    }

    public async Task MarkSetupTokenAsUsed(string tokenHash, CancellationToken cancellationToken)
    {
        var setupToken = await this.db.TenantSetupTokens
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken)
            ?? throw new NotFoundException(nameof(TenantSetupToken), tokenHash);

        setupToken.UsedAt = DateTime.UtcNow;
        await this.db.SaveChangesAsync(cancellationToken);
    }

    async Task CreateSeededGames(string tenantId, List<SeedGameCatalog> selectedSeeds, CancellationToken cancellationToken)
    {
        var categoryByName = new Dictionary<string, GameCategory>(StringComparer.OrdinalIgnoreCase);

        foreach (var seed in selectedSeeds)
        {
            if (!categoryByName.TryGetValue(seed.CategoryName, out var category))
            {
                category = new GameCategory
                {
                    Name = seed.CategoryName,
                    TenantId = tenantId,
                };

                categoryByName[seed.CategoryName] = category;
                await this.db.GameCategories.AddAsync(category, cancellationToken);
            }

            var game = new Game
            {
                TenantId = tenantId,
                Name = seed.Name,
                Description_EN = seed.Description_EN,
                Description_BG = seed.Description_BG,
                MinAge = seed.MinAge,
                MinPlayers = seed.MinPlayers,
                MaxPlayers = seed.MaxPlayers,
                AveragePlaytime = seed.AveragePlaytime,
                ImageUrl = ResolveSeedImageUrl(seed),
                Category = category,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };

            await this.db.Games.AddAsync(game, cancellationToken);
            await this.db.GameInventories.AddAsync(new GameInventory
            {
                TenantId = tenantId,
                Game = game,
                AvailableCopies = 1,
                TotalCopies = 1,
            }, cancellationToken);
        }
    }

    string ResolveSeedImageUrl(SeedGameCatalog seed)
    {
        if (!string.IsNullOrWhiteSpace(seed.ImageUrl))
            return seed.ImageUrl;

        if (string.IsNullOrWhiteSpace(seed.ImageFileName))
            return string.Empty;

        return this.fileManagerClient.GetPublicUrl(FileManagerFolders.Seed.ToString(), seed.ImageFileName);
    }

    async Task<string> CreateUniqueTenantId(string clubName, CancellationToken cancellationToken)
    {
        var baseSlug = Slugify(clubName);
        var slug = baseSlug;
        var suffix = 2;

        while (await this.db.Tenants.AnyAsync(x => x.Id == slug, cancellationToken))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    static string Slugify(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        var slug = Regex.Replace(normalized, @"[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? $"tenant-{DateTime.UtcNow:yyyyMMddHHmmss}" : slug;
    }

    static void ValidateRequest(CompleteTenantSetupRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Token))
            errors[nameof(request.Token)] = ["Setup token is required."];
        if (string.IsNullOrWhiteSpace(request.ClubName))
            errors[nameof(request.ClubName)] = ["Club name is required."];
        if (request.AverageMaxCapacity <= 0)
            errors[nameof(request.AverageMaxCapacity)] = ["Average max capacity must be greater than zero."];
        if (string.IsNullOrWhiteSpace(request.StartWorkingHours))
            errors[nameof(request.StartWorkingHours)] = ["Start working hours are required."];
        if (string.IsNullOrWhiteSpace(request.EndWorkingHours))
            errors[nameof(request.EndWorkingHours)] = ["End working hours are required."];
        if (string.IsNullOrWhiteSpace(request.ClubPhoneNumber))
            errors[nameof(request.ClubPhoneNumber)] = ["Club phone number is required."];
        if (request.SelectedGameIds.Count == 0)
            errors[nameof(request.SelectedGameIds)] = ["Select at least one starting game."];

        if (errors.Count > 0)
            throw new ValidationErrorsException(errors);
    }

    private sealed class TenantSetupSystemUserContext(string tenantId) : IUserContext
    {
        public string? TenantId => tenantId;
        public string? UserId => "tenant-setup";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
