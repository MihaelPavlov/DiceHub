using DH.Adapter.Authentication.Entities;
using DH.Adapter.Authentication.Helper;
using DH.Application.Emails.Commands;
using DH.Application.Games.Commands.Games;
using DH.Application.Rewards.Commands;
using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Authentication.Helpers;
using DH.Domain.Adapters.Authentication.Models.Enums;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Data;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.Domain.Services.TenantSettingsService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DH.Api;

public class TenantProvisioningService
{
    readonly ITenantService tenantService;
    readonly IRepository<TenantSetting> tenantSettingsRepository;
    readonly IRepository<GameCategory> gameCategoryRepository;
    readonly IRepository<UniversalChallenge> universalChallengeRepository;
    readonly IRepository<EmailTemplate> emailTemplateRepository;
    readonly IRepository<Game> gameRepository;
    readonly IRepository<ChallengeReward> rewardRepository;
    readonly UserManager<ApplicationUser> userManager;
    readonly RoleManager<IdentityRole> roleManager;
    readonly IUserContext userContext;
    readonly IUserManagementService userManagementService;
    readonly ITenantExecutionContextAccessor tenantExecutionContextAccessor;
    readonly ISystemUserContextAccessor systemUserContextAccessor;
    readonly ITenantSettingsCacheService tenantSettingsCacheService;
    readonly IMediator mediator;

    public TenantProvisioningService(
        ITenantService tenantService,
        IRepository<TenantSetting> tenantSettingsRepository,
        IRepository<GameCategory> gameCategoryRepository,
        IRepository<UniversalChallenge> universalChallengeRepository,
        IRepository<EmailTemplate> emailTemplateRepository,
        IRepository<Game> gameRepository,
        IRepository<ChallengeReward> rewardRepository,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserContext userContext,
        IUserManagementService userManagementService,
        ITenantExecutionContextAccessor tenantExecutionContextAccessor,
        ISystemUserContextAccessor systemUserContextAccessor,
        ITenantSettingsCacheService tenantSettingsCacheService,
        IMediator mediator)
    {
        this.tenantService = tenantService;
        this.tenantSettingsRepository = tenantSettingsRepository;
        this.gameCategoryRepository = gameCategoryRepository;
        this.universalChallengeRepository = universalChallengeRepository;
        this.emailTemplateRepository = emailTemplateRepository;
        this.gameRepository = gameRepository;
        this.rewardRepository = rewardRepository;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.userContext = userContext;
        this.userManagementService = userManagementService;
        this.tenantExecutionContextAccessor = tenantExecutionContextAccessor;
        this.systemUserContextAccessor = systemUserContextAccessor;
        this.tenantSettingsCacheService = tenantSettingsCacheService;
        this.mediator = mediator;
    }

    public async Task<CreateTenantResult> ProvisionAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        if (!await this.userManagementService.HasUserAnyMatchingRole(this.userContext.UserId!, Role.SuperAdmin))
            throw new UnauthorizedAccessException("Only super admins can provision tenants.");

        if (string.IsNullOrWhiteSpace(request.TenantId))
            throw new ArgumentException("TenantId is required.");

        if (string.IsNullOrWhiteSpace(request.TenantName))
            throw new ArgumentException("TenantName is required.");

        if (string.IsNullOrWhiteSpace(request.OwnerEmail))
            throw new ArgumentException("OwnerEmail is required.");

        if (await this.tenantService.GetById(request.TenantId.Trim()) != null)
            throw new InvalidOperationException($"Tenant '{request.TenantId}' already exists.");

        if (await this.tenantService.GetByTenantName(request.TenantName.Trim()) != null)
            throw new InvalidOperationException($"Tenant name '{request.TenantName}' is already in use.");

        var tenantSetting = await this.tenantSettingsRepository.AddAsync(BuildDefaultTenantSettings(request), cancellationToken);
        var tenant = await this.tenantService.CreateAsync(new Tenant
        {
            Id = request.TenantId.Trim(),
            TenantName = request.TenantName.Trim(),
            Town = request.Town.Trim(),
            TenantStatus = TenantStatus.Active,
            CreatedDate = DateTime.UtcNow,
            LogoFileName = request.LogoFileName.Trim(),
            RegisterQrCode = string.Empty,
            TenantSettingId = tenantSetting.Id
        }, cancellationToken);

        SetTenantExecutionContext(tenant.Id);
        try
        {
            await this.ApplyStarterProfileAsync(request.StarterProfile, cancellationToken);
            await this.CreateOwnerAsync(request, tenant.Id, cancellationToken);
            this.tenantSettingsCacheService.Clear(tenant.Id);
        }
        finally
        {
            ClearTenantExecutionContext();
        }

        return new CreateTenantResult
        {
            TenantId = tenant.Id,
            OwnerEmail = request.OwnerEmail,
            StarterProfile = request.StarterProfile
        };
    }

    private async Task ApplyStarterProfileAsync(string starterProfile, CancellationToken cancellationToken)
    {
        if (string.Equals(starterProfile, "empty-club", StringComparison.OrdinalIgnoreCase))
            return;

        var sourceCategories = DH.Domain.Adapters.Data.SeedData.GAME_CATEGORIES;
        List<GameCategory> categories;

        if ((await this.gameCategoryRepository.GetWithPropertiesAsync(x => x.Id, cancellationToken)).Count == 0)
        {
            categories = sourceCategories
                .Select(x => new GameCategory { Name = x.Name })
                .ToList();
            await this.gameCategoryRepository.AddRangeAsync(categories, cancellationToken);
        }
        else
        {
            categories = await this.gameCategoryRepository.GetWithPropertiesAsync(
                x => new GameCategory { Id = x.Id, Name = x.Name },
                cancellationToken);
        }

        var categoryMap = sourceCategories.ToDictionary(
            x => x.Id,
            x => categories.First(c => c.Name == x.Name).Id);

        if ((await this.universalChallengeRepository.GetWithPropertiesAsync(x => x.Id, cancellationToken)).Count == 0)
        {
            await this.universalChallengeRepository.AddRangeAsync(
                DH.Domain.Adapters.Data.SeedData.UNIVERSAL_CHALLENGES.Select(x => new UniversalChallenge
                {
                    RewardPoints = x.RewardPoints,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    CreatedBy = "TenantProvisioning",
                    UpdatedBy = "TenantProvisioning",
                    Name_EN = x.Name_EN,
                    Name_BG = x.Name_BG,
                    Description_EN = x.Description_EN,
                    Description_BG = x.Description_BG,
                    Type = x.Type,
                    Attempts = x.Attempts,
                    MinValue = x.MinValue
                }),
                cancellationToken);
        }

        if ((await this.emailTemplateRepository.GetWithPropertiesAsync(x => x.Id, cancellationToken)).Count == 0)
        {
            await this.emailTemplateRepository.AddRangeAsync(
                DH.Domain.Adapters.Data.SeedData.EMAIL_TEMPLATES.Select(x => new EmailTemplate
                {
                    Language = x.Language,
                    TemplateName = x.TemplateName,
                    TemplateHtml = x.TemplateHtml,
                    Subject = x.Subject
                }),
                cancellationToken);
        }

        if ((await this.rewardRepository.GetWithPropertiesAsync(x => x.Id, cancellationToken)).Count == 0)
        {
            foreach (var reward in DH.Domain.Adapters.Data.SeedData.REWARD_LIST_DTOS)
            {
                if (string.IsNullOrWhiteSpace(reward.Name_BG))
                    reward.Name_BG = reward.Name_EN;
                await this.mediator.Send(new CreateSystemRewardCommand(reward, "reward_image.png", "image/png", new MemoryStream()), cancellationToken);
            }
        }

        if ((await this.gameRepository.GetWithPropertiesAsync(x => x.Id, cancellationToken)).Count == 0)
        {
            foreach (var game in DH.Domain.Adapters.Data.SeedData.GAME_LIST_DTOS)
            {
                var gameDto = new DH.Domain.Models.GameModels.Commands.CreateGameDto
                {
                    CategoryId = categoryMap[game.CategoryId],
                    Name = game.Name,
                    Description_EN = game.Description_EN,
                    Description_BG = game.Description_BG,
                    MinAge = game.MinAge,
                    MinPlayers = game.MinPlayers,
                    MaxPlayers = game.MaxPlayers,
                    AveragePlaytime = game.AveragePlaytime
                };

                await this.mediator.Send(new CreateGameCommand(gameDto, "game_image.png", "image/png", new MemoryStream()), cancellationToken);
            }
        }
    }

    private async Task CreateOwnerAsync(CreateTenantRequest request, string tenantId, CancellationToken cancellationToken)
    {
        if (await this.userManager.FindByEmailAsync(request.OwnerEmail) != null)
            throw new InvalidOperationException($"User '{request.OwnerEmail}' already exists.");

        if (!await this.roleManager.RoleExistsAsync(Role.Owner.ToString()))
            await this.roleManager.CreateAsync(new IdentityRole(Role.Owner.ToString()) { Id = ((int)Role.Owner).ToString() });

        var user = new ApplicationUser
        {
            UserName = request.OwnerEmail,
            Email = request.OwnerEmail,
            PhoneNumber = request.ClubPhoneNumber,
            EmailConfirmed = true,
            TenantId = tenantId
        };

        var generatedPassword = PasswordGenerator.GenerateRandomPassword();
        var createUserResult = await this.userManager.CreateAsync(user, generatedPassword);
        if (!createUserResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", createUserResult.Errors.Select(x => x.Description)));

        await this.userManager.AddToRoleAsync(user, Role.Owner.ToString());
        await this.mediator.Send(new SendOwnerCreatePasswordEmailCommand(request.OwnerEmail), cancellationToken);
    }

    private static TenantSetting BuildDefaultTenantSettings(CreateTenantRequest request)
    {
        return new TenantSetting
        {
            AverageMaxCapacity = 24,
            ChallengeRewardsCountForPeriod = 4,
            PeriodOfRewardReset = TimePeriodType.Weekly.ToString(),
            ResetDayForRewards = WeekDays.Monday.ToString(),
            DaysOff = string.Empty,
            StartWorkingHours = "10:00",
            EndWorkingHours = "22:00",
            ChallengeInitiationDelayHours = 2,
            ReservationHours = "10:00,12:00,14:00",
            BonusTimeAfterReservationExpiration = 15,
            PhoneNumber = request.ClubPhoneNumber,
            ClubName = request.TenantName,
            IsCustomPeriodOn = false,
            IsCustomPeriodSetupComplete = false
        };
    }

    private void SetTenantExecutionContext(string tenantId)
    {
        this.tenantExecutionContextAccessor.TenantId = tenantId;
        this.systemUserContextAccessor.Set(new SystemUserContext(tenantId, "system-provisioner"));
    }

    private void ClearTenantExecutionContext()
    {
        this.tenantExecutionContextAccessor.Clear();
    }
}
