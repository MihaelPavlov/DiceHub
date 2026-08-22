# Graph Report - DiceHub  (2026-08-22)

## Corpus Check
- 1180 files · ~5,990,622 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 7202 nodes · 18551 edges · 362 communities (300 shown, 62 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 549 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `4ff641a0`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- AuthService
- shared.module.ts
- ControlsMenuComponent
- TenantDbContext
- TenantRouter
- IRequest
- ToastService
- IRepository
- DH.Domain.Entities
- GamesController
- .error
- DH.OperationResultCore.Exceptions
- ILocalizationService
- IStatisticsService
- IUserContext
- DH.Domain.Adapters.Localization
- admin-challenges-custom-period.component.ts
- OperationResult
- GlobalSettingsComponent
- ISystemUserContextAccessor
- QRReaderModel
- RoomsController
- IUserManagementService
- TenantSetupService
- IChallengeService
- GameNavigationComponent
- IPushNotificationsService
- .post
- qr-code-scanner.component.ts
- DH.Domain.Repositories
- DH.Domain.Adapters.Data
- NotificationsDialog
- LinkInfoComponent
- ClubSpaceDetailsComponent
- UserChallengesManagementService
- .navigateTenant
- DH.Domain.Adapters.Authentication
- statistics.service.ts
- DH.Statistics.Domain.Entities
- MessagingService
- .get
- .getCurrentLanguage
- UserController
- ReservationCleanupWorker
- DH.Database.Connector
- DH.Statistics.Domain.Models.Queries
- .GetGlobalTenantSettingsAsync
- StatisticsService
- AdminChallengesCustomPeriodComponent
- LanguageService
- GetExpiredCollectedRewardsChartDataModel
- ISchedulerService
- AdminChallengesSystemRewardsComponent
- RewardsController
- SpaceManagementController
- RestApiService
- AuthenticationService
- .Update
- AuthorizedHttpClient
- DH.Domain.Models.Common
- DH.Messaging.Publisher
- TenantContextService
- EmployeeService
- OwnerService
- http
- rooms.service.ts
- RoomChatComponent
- DH.Domain.Models.ChallengeModels.Queries
- ChallengesManagementComponent
- dependencies
- devDependencies
- DH.DiceHub/DH.Domain/DH.Domain.csproj
- EventsLibraryComponent
- AddUpdateEventComponent
- TenantIsolationFixture
- DH.Domain.Queue
- UniversalChallengeProcessing
- IReservationCleanupQueue
- IGameSessionQueue
- StatisticController
- challenges.service.ts
- PushNotificationsService
- DH.Database.MigrationUtility
- ChallengesController
- .getClubName
- IRabbitMqUserContext
- ControllerBase
- .UpdateSettings
- IValidableFields
- AddUpdateClubSpaceComponent
- app.module.ts
- SpaceBookingComponent
- DH.Domain.Adapters.Scheduling
- .AddEmailAdapter
- UserManagementService
- EmailType.cs
- .RefreshAccessTokenAsync
- QueuedJob
- TenantSetting
- IStatisticJobInfo
- HeaderComponent
- .buildTenantUrl
- DH.Statistics.Data
- AddUpdateGameComponent
- GetSeedGameCatalogDropdownListQueryHandler
- GetGameReservedListQueryHandler
- AssistiveTouchComponent
- ConsoleFileLogger
- TenantDbConnectionInterceptor
- .EnsureRoleAsync
- DataRepository
- GameService
- SendOwnerCreatePasswordEmailCommandHandler
- DataSeeder
- IUniversalChallengeProcessing
- AppIdentityDbContext
- AuthorizedClientFactory
- IGameService
- Chart2Component
- IUserContext
- IRabbitMqClient
- StatisticsController
- ISeedService
- GetGameListQueryModel
- DH.Authentication.UserContext.csproj
- IQueuedJobService
- DH.Statistics.WorkerService.csproj
- RabbitMqWorker
- VisitorsChartComponent
- LoadingIndicatorComponent
- .UploadQrCode
- DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj
- DH.Statistics.Application/Queries/GetActivityChartDataQuery.cs
- ApiExceptionFilterAttribute
- TenantApplicationDto
- DH.Database.MigrationUtility.csproj
- GetEventListQueryModel
- scripts
- LandingComponent
- MeepleRoomMenuComponent
- IRoomService
- VenueApplicationComponent
- Tenant Isolation Plan
- AppComponent
- EventService
- SchedulerService
- GetTenantListQueryModel
- GetSpaceActivityStatsQueryHandler
- DH.Database.Connector.csproj
- .ChangePassword
- .CreateHostBuilder
- SendTenantApplicationEmailVerificationCodeCommandHandler
- DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs
- options
- CreateEmployeePasswordComponent
- CreateOwnerPasswordComponent
- SendTenantSetupInvitationCommandHandler
- SpaceTableActiveReservations
- ReservationHistoryActionsComponent
- 20250115170446_InitialSeedQuartzNET.Designer.cs
- Google Cloud Setup and Deployment Notes
- NotificationsController
- VerifyTenantApplicationEmailVerificationCodeCommandHandler
- .GetUserLocalOrUtcTime
- ReservationType
- AssistiveTouchComponent
- QRCodeContext
- ChallengeType
- EventAttendanceByEventsChartComponent
- GameLayoutComponent
- GetRoomListQueryHandler
- ISynchronizeUsersChallengesQueue
- ISpaceTableService
- IDomainService
- MapPermissions
- GetExpiredCollectedRewardsChartDataQuery
- GameReservationHistory
- SpaceTableReservationHistory
- .resetData
- EventAttendanceChartComponent
- TokenService
- .ValidateQRCodeAsync
- DH.DiceHub.sln
- DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj
- TenantDbContext
- http
- GetCustomPeriodQueryModel
- GetActivityChartData
- GameSessionQueue
- EmailHelperService
- AdminUniversalChallengesComponent
- GamesChartComponent
- manifest.json
- ActionAuthorizeFilter
- .SubmitInquiry
- DH.Adapter.Authentication.Migrations
- IAddUserChallengePeriodHandler
- IJob
- IUserRewardsExpirationReminderHandler
- IUserRewardsExpiryHandler
- UserSettingsDto
- GetGameActivityChartData
- GetUserWhoPlayedGameChartDataQueryHandler
- RoomService
- .TryDequeue
- QrCodeValidationResult
- AuthTokenService
- GetGameReviewListQueryHandler
- AdminEventDetailsComponent
- .Handle
- ReservationsChartComponent
- DH.Domain.Adapters.Statistics.Services
- ChallengeHubClientProxy
- GetGameReservationHistoryQueryHandler
- SpaceTableService
- DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj
- GetUserRewardListQueryHandler
- GetRoomInfoMessageListQueryHandler
- GetRoomMessageListQueryHandler
- GetUserActiveTableQueryHandler
- DH.Messaging.Publisher.csproj
- ChallengeProcessingOutcomeMessage
- GetReservationChartDataQuery
- development
- production
- DH.Adapter.FileManager
- GetActiveReservedGameQueryHandler
- RewardsCollectedChartComponent
- ScrollTopComponent
- ChatHubClient.cs
- PermissionStringBuilder
- AppIdentityDbContextModelSnapshot
- .JobWasExecuted
- ITenantSettingsCacheService
- GetAllEventsDropdownListQueryHandler
- GetActiveGameReservationListQueryHandler
- GetGameReservationStatusQueryHandler
- GetSystemRewardDropdownListQueryHandler
- GetUserChallengePeriodRewardListQueryHandler
- GetActiveSpaceTableReservationListQueryHandler
- GetSpaceAvailableTableListQuery
- GetSpaceTableParticipantListQueryHandler
- GetGameReservationByIdQueryHandler
- SynchronizeUsersChallengesQueue
- DH.WebUI
- DHWebUI
- ForgotPasswordComponent
- ToastComponent
- SupabaseStorageClient
- DH.Statistics.WorkerService.Common
- ChallengeReward
- DH.Adapter.Email
- .GetGameCategoryList
- GetChallengeListWithFilterQuery
- GetActiveBookedSpaceTableQueryHandler
- GetUserChallengePeriodPerformanceQueryHandler
- GetAssistiveTouchSettingsQueryHandler
- GetEventByIdQueryModel
- GetGameInventoryQueryHandler
- GetSpaceTableReservationByIdQueryHandler
- IGameSessionService
- GetChallengeByIdQueryHandler
- DH.DiceHub.IntegrationTests
- angular.json
- architect
- StreakComponent
- RandomColorDirective
- IPermissionStringBuilder
- DH.Adapter.FileManager
- .AddSchedulingAdapter
- DeleteGameCommandHandler
- GetActiveGameReservationCountQueryHandler
- GetSystemRewardByIdQueryHandler
- DeleteRoomCommandHandler
- assets
- StreakRewardsComponent
- UpdateEventModel
- PasswordVisibilityToggleComponent
- ApiEndpoints.cs
- Migration
- DH.Adapter.ChallengeHub
- .AddDataAdapter
- InitialSeedQuartzNET
- InitialTenant
- InitialData
- AddTenantApplications
- AddSeedGameCatalog
- AddTenantSetupTokens
- FixSeedGameCatalogCategories
- AddTenantApplicationLink
- DH.Adapter.Localization
- DH.Adapter.PushNotifications
- DH.Adapter.Statistics
- .GetActiveUserCustomPeriod
- CreateGameReviewDto
- UpdateGameReviewDto
- CreateRoomCommandDto
- UpdateRoomCommandDto
- InitialCreate
- DiceRollerComponent
- NavBarComponent
- DH.Adapter.Data.Migrations
- UpdateRewardDto
- CalculateRemainingDaysPipe
- ChatHubClient
- ChipComponent
- 20260729093650_AddSeedGameCatalog.Designer.cs
- Models/Common/RabbitMqOptions.cs
- 20260729094735_AddTenantSetupTokens.Designer.cs
- .HandleAsync
- ExampleInstrumentedTest
- gradlew
- BridgeActivity
- GameComplexDataQuery.cs
- DH.Domain.Adapters.Email.Models
- .HandleAsync
- ExampleUnitTest
- DiceHub Design Mockups
- AGENTS.md
- @angular/common
- @angular/core
- @angular/fire
- @angular/platform-browser-dynamic
- angularx-qrcode
- @auth0/angular-jwt
- @capacitor/core
- @capacitor-firebase/messaging
- chartjs-plugin-datalabels
- CLAUDE.md
- crypto-js
- DH.DiceHub/deploy.sh
- UpdateChallengeDto.cs
- capacitor.config.ts
- DH.WebUI/deploy.sh
- jsqr
- memoize-one
- @microsoft/signalr
- @ng-select/ng-select
- @ngx-translate/core
- rxjs
- tslib
- zone.js
- challenge-dropdown.model.ts
- game-qr-code.model.ts
- tenant-settings.interface.ts
- environment.prod.ts
- README.md
- ReservationOutcomeLog
- CanComponentDeactivate

## God Nodes (most connected - your core abstractions)
1. `DH.Domain.Entities` - 221 edges
2. `TenantRouter` - 165 edges
3. `DH.Domain.Enums` - 127 edges
4. `DH.Domain.Repositories` - 116 edges
5. `ToastService` - 116 edges
6. `IRepository` - 113 edges
7. `AuthService` - 110 edges
8. `DH.OperationResultCore.Exceptions` - 107 edges
9. `DH.Domain.Adapters.Localization` - 101 edges
10. `ILocalizationService` - 100 edges

## Surprising Connections (you probably didn't know these)
- `WorkerSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.ChallengesOrchestrator/SynchronizeUsersChallengesWorker.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `DataSeederSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/DataSeeder.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `QueuedJobSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/Services/QueuedJobService.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `TenantSetupSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/Services/TenantSetupService.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `TenantOwnerCredentialsSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Application/Common/Commands/SendTenantOwnerCredentialsEmailCommand.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs

## Import Cycles
- None detected.

## Communities (362 total, 62 thin omitted)

### Community 0 - "AuthService"
Cohesion: 0.04
Nodes (48): TODO: Check this tread…, AuthService, Injectable, UserRole, IRegisterRequest, IRegisterResponse, ITokenResponse, IUserInfo (+40 more)

### Community 1 - "shared.module.ts"
Cohesion: 0.03
Nodes (63): AdminChallengesHistoryLogComponent, Component, CustomPeriodLeaveConfirmationDialog, Component, SinglePlayerConfirmDialog, Component, Inject, EventConfirmDeleteDialog (+55 more)

### Community 2 - "ControlsMenuComponent"
Cohesion: 0.09
Nodes (8): AdminEventManagementComponent, Component, GamesLibraryComponent, Component, ControlsMenuComponent, Component, Input, Output

### Community 3 - "TenantDbContext"
Cohesion: 0.03
Nodes (77): CancellationToken, DbContextOptionsBuilder, DbSet, IHttpContextAccessor, ModelBuilder, Task, TenantDbContext, IConfiguration (+69 more)

### Community 4 - "TenantRouter"
Cohesion: 0.04
Nodes (44): EventsService, Injectable, GameCategoriesService, Injectable, GamesService, Injectable, IGameCategory, IGameListResult (+36 more)

### Community 5 - "IRequest"
Cohesion: 0.03
Nodes (77): CreateChallengeCommand, CreateChallengeCommandHandler, CancellationToken, Task, SaveCustomPeriodCommand, SaveCustomPeriodCommandHandler, CancellationToken, List (+69 more)

### Community 6 - "ToastService"
Cohesion: 0.07
Nodes (54): IGameByIdResult, IGameDropdownResult, IChallengeForm, ISystemRewardsForm, IChallengeLeaderboardData, TODO: LOCALIZATION name_en, Colors, ColorShades (+46 more)

### Community 7 - "IRepository"
Cohesion: 0.04
Nodes (70): CancellationToken, Task, ExpiredRewardInfo, UserRewardsExpiryHandler, CancellationToken, Task, UpdateUniversalChallengeCommand, UpdateUniversalChallengeCommandHandler (+62 more)

### Community 8 - "DH.Domain.Entities"
Cohesion: 0.06
Nodes (18): DH.Adapter.Data.Services, DH.Adapter.Email, DH.Domain.Adapters.FileManager, DH.Domain.Adapters.Email, DH.Domain.Entities, DH.Domain.Services.TenantSettingsService, DH.Domain.Enums, DH.Application.Emails.Commands (+10 more)

### Community 9 - "GamesController"
Cohesion: 0.26
Nodes (12): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+4 more)

### Community 10 - ".error"
Cohesion: 0.03
Nodes (4): IChangePasswordRequest, IUpdateEventDto, IAddUpdateRoomDto, IUpdateSpaceTableDto

### Community 11 - "DH.OperationResultCore.Exceptions"
Cohesion: 0.06
Nodes (22): DH.Domain.Adapters.QRManager.StateModels, DH.OperationResultCore.Exceptions, DH.Domain.Adapters.Statistics, DH.Application.SpaceManagement.Commands, DH.Adapter.Statistics, DH.Domain.Adapters.Reservations, DH.Domain.Adapters.GameSession, DH.Domain.Models.SpaceManagementModels.Commands (+14 more)

### Community 12 - "ILocalizationService"
Cohesion: 0.04
Nodes (56): LocalizationService, GameQRCodeState, GameReservationQRCodeState, PurchaseChallengeQRCodeState, RewardQRCodeState, TableReservationQRCodeState, ILocalizationService, ChallengeCompletedNotification (+48 more)

### Community 13 - "IStatisticsService"
Cohesion: 0.04
Nodes (58): CancellationToken, Task, GetEventAttendanceByIdsQuery, GetEventAttendanceByIdsQueryHandler, CancellationToken, Task, GetEventAttendanceChartDataQuery, GetEventAttendanceChartDataQueryHandler (+50 more)

### Community 14 - "IUserContext"
Cohesion: 0.04
Nodes (47): SystemUserContextAccessor, UserContext, IHttpContextAccessor, Task, UserContextFactory, IMemoryCache, Task, UserSettingsCache (+39 more)

### Community 15 - "DH.Domain.Adapters.Localization"
Cohesion: 0.06
Nodes (21): DH.Adapter.ChallengeHub, DH.Domain.Adapters.PushNotifications.Messages.Models, DH.Domain.Adapters.PushNotifications, DH.Domain.Adapters.Localization, DH.Domain.Adapters.ChallengeHub, DH.Adapter.PushNotifications, DH.Domain.Adapters.PushNotifications.Messages.Common, DH.Domain.Adapters.PushNotifications.Messages (+13 more)

### Community 16 - "admin-challenges-custom-period.component.ts"
Cohesion: 0.04
Nodes (39): ICustomPeriodChallenge, ICustomPeriodReward, ICustomPeriodUniversalChallenge, IUserChallengePeriodPerformance, TenantSettingsService, Injectable, TimePeriodType, ITenantSettings (+31 more)

### Community 17 - "OperationResult"
Cohesion: 0.06
Nodes (32): DH.OperationResultCore.Extension, CancellationToken, List, Task, GetChallengeHistoryLogQuery, GetChallengeHistoryLogQueryHandler, CancellationToken, List (+24 more)

### Community 18 - "GlobalSettingsComponent"
Cohesion: 0.09
Nodes (5): ToggleState, GlobalSettingsComponent, Component, Component, UserSettingsComponent

### Community 19 - "ISystemUserContextAccessor"
Cohesion: 0.08
Nodes (27): CancellationToken, IConfiguration, ILogger, Task, SendTenantOwnerCredentialsEmailCommand, SendTenantOwnerCredentialsEmailCommandHandler, TenantOwnerCredentialsSystemUserContext, CancellationToken (+19 more)

### Community 20 - "QRReaderModel"
Cohesion: 0.15
Nodes (15): CancellationToken, Task, CancellationToken, Task, CancellationToken, Task, CancellationToken, Task (+7 more)

### Community 21 - "RoomsController"
Cohesion: 0.34
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 22 - "IUserManagementService"
Cohesion: 0.15
Nodes (12): CancellationToken, IConfiguration, ILogger, Task, RegistrationEmailSystemUserContext, SendRegistrationEmailConfirmationCommand, SendRegistrationEmailConfirmationCommandHandler, UserModel (+4 more)

### Community 23 - "TenantSetupService"
Cohesion: 0.05
Nodes (49): CancellationToken, List, Task, TenantDbContext, TenantSetupService, TenantSetupSystemUserContext, ActionAuthorize, AllowAnonymous (+41 more)

### Community 24 - "IChallengeService"
Cohesion: 0.06
Nodes (43): CancellationToken, IDbContextFactory, List, Task, ChallengeService, CancellationToken, Task, CancellationToken (+35 more)

### Community 25 - "GameNavigationComponent"
Cohesion: 0.13
Nodes (4): GameCategoriesComponent, Component, GameNavigationComponent, Component

### Community 26 - "IPushNotificationsService"
Cohesion: 0.06
Nodes (35): ConcurrentDictionary, Exception, IHubContext, Task, ChallengeHubClient, CancellationToken, Task, UserRewardsExpirationReminderHandler (+27 more)

### Community 27 - ".post"
Cohesion: 0.07
Nodes (19): TenantApplicationsService, Injectable, ICompleteTenantSetupRequest, ICompleteTenantSetupResult, ISeedGameCatalogDropdown, ITenantApplication, ITenantApplicationRequest, ITenantApplicationReviewRequest (+11 more)

### Community 28 - "qr-code-scanner.component.ts"
Cohesion: 0.05
Nodes (31): ScannerService, Injectable, QrCodeType, IQrCode, IQrCodeRequest, IQrCodeValidationResult, Component, Inject (+23 more)

### Community 29 - "DH.Domain.Repositories"
Cohesion: 0.06
Nodes (13): DH.Domain.Models.RoomModels.Commands, DH.Domain.Models.EventModels.Queries, DH.Domain.Models.GameModels.Queries, DH.Domain.Repositories, DH.Domain.Models.RewardModels.Commands, DH.Application.Games.Queries, DH.Application.Events.Queries, DH.Domain.Models.RewardModels.Queries (+5 more)

### Community 30 - "DH.Domain.Adapters.Data"
Cohesion: 0.03
Nodes (34): DH.Adapter.Data.Repositories, DH.Application, DH.Application.Games.Seeders, DH.Application.Games.Commands.Games, DH.Api, DH.Adapter.Data, DH.Domain, DH.Adapter.Data.Seeder (+26 more)

### Community 31 - "NotificationsDialog"
Cohesion: 0.11
Nodes (7): NotificationsService, Injectable, IUserNotification, NotificationsDialog, Component, Inject, ViewChild

### Community 32 - "LinkInfoComponent"
Cohesion: 0.06
Nodes (22): INSTRUCTION_LINK_MAPPINGS, InstructionSection, InstructionStep, InstructionTopic, LinkInfoType, StepActionLink, InstructionComponent, Component (+14 more)

### Community 34 - "UserChallengesManagementService"
Cohesion: 0.14
Nodes (18): DbUpdateException, CancellationToken, IDbContextFactory, IDbContextTransaction, ILogger, List, Task, TenantDbContext (+10 more)

### Community 35 - ".navigateTenant"
Cohesion: 0.03
Nodes (15): EventsChartsLayoutComponent, Component, RewardChartsLayoutComponent, Component, EmployeeListComponent, Component, ClubSpaceManagementComponent, Component (+7 more)

### Community 36 - "DH.Domain.Adapters.Authentication"
Cohesion: 0.09
Nodes (13): DH.Adapter.Authentication.Helper, DH.Domain.Models, DH.Domain.Adapters.Authentication.Options, DH.Application.Rooms.Queries, DH.Domain.Adapters.Authentication.Interfaces, DH.Adapter.Authentication.Entities, DH.Domain.Adapters.Authentication, DH.Domain.Adapters.Authentication.Models (+5 more)

### Community 37 - "statistics.service.ts"
Cohesion: 0.06
Nodes (20): ChallengeLeaderboardType, ChartActivityType, GamesActivityType, ActivityLog, GetActivityChartData, IChallengeLeaderboard, GetCollectedRewardsByDates, EventAttendance (+12 more)

### Community 38 - "DH.Statistics.Domain.Entities"
Cohesion: 0.07
Nodes (24): DH.Statistics.Domain.Enums, DH.Statistics.Domain.Entities, CancellationToken, IDbContextFactory, Task, CreateReservationOutcomeCommand, CreateReservationOutcomeCommandHandler, DbSet (+16 more)

### Community 39 - "MessagingService"
Cohesion: 0.08
Nodes (6): MessagingService, Injectable, LoginComponent, Component, RegisterComponent, Component

### Community 40 - ".get"
Cohesion: 0.05
Nodes (11): ChallengesService, Injectable, GetOwnerStats, GetUserStats, IOwnerResult, IUser, AdminChallengesListComponent, Component (+3 more)

### Community 41 - ".getCurrentLanguage"
Cohesion: 0.11
Nodes (4): ChallengeHubService, Injectable, ChallengeOverlayComponent, Component

### Community 42 - "UserController"
Cohesion: 0.19
Nodes (18): ActionAuthorize, AllowAnonymous, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut (+10 more)

### Community 43 - "ReservationCleanupWorker"
Cohesion: 0.07
Nodes (30): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, SynchronizeUsersChallengesWorker, WorkerSystemUserContext, CancellationToken (+22 more)

### Community 44 - "DH.Database.Connector"
Cohesion: 0.11
Nodes (11): DH.Database.Connector, Assembly, IConfiguration, IServiceCollection, DI, Assembly, string, TenantDbContextFactory (+3 more)

### Community 45 - "DH.Statistics.Domain.Models.Queries"
Cohesion: 0.08
Nodes (30): DH.Statistics.Application.Queries, DH.Statistics.Api.Controllers, DH.Statistics.Domain.Models.Queries, CancellationToken, IDbContextFactory, List, Task, GetCollectedRewardsByDatesQuery (+22 more)

### Community 46 - ".GetGlobalTenantSettingsAsync"
Cohesion: 0.14
Nodes (21): completedChallenge, completedUniversalChallenges, CancellationToken, IDbContextFactory, IDbContextTransaction, IEnumerable, ILogger, List (+13 more)

### Community 47 - "StatisticsService"
Cohesion: 0.10
Nodes (16): CancellationToken, DateTime, IDbContextFactory, List, Task, StatisticsService, Test, ChallengeHistoryLogType (+8 more)

### Community 48 - "AdminChallengesCustomPeriodComponent"
Cohesion: 0.07
Nodes (5): ICustomPeriod, IUniversalChallengeDropdownResult, AdminChallengesCustomPeriodComponent, customPeriodValidator(), Component

### Community 49 - "LanguageService"
Cohesion: 0.04
Nodes (48): SupportLanguages, ActiveReservedGame, ICreateGameReservation, IGameInventory, IGameReservationStatus, IGetReservationById, IReservedGame, SpaceManagementService (+40 more)

### Community 50 - "GetExpiredCollectedRewardsChartDataModel"
Cohesion: 0.33
Nodes (7): CancellationToken, Task, GetExpiredCollectedRewardsChartDataQuery, GetExpiredCollectedRewardsChartDataQueryHandler, List, GetExpiredCollectedRewardsChartDataModel, RewardsStats

### Community 51 - "ISchedulerService"
Cohesion: 0.09
Nodes (22): CancellationToken, Task, AddUserChallengePeriodHandler, IJobExecutionContext, ILogger, Task, UserChallengeValidationJob, ActionAuthorize (+14 more)

### Community 52 - "AdminChallengesSystemRewardsComponent"
Cohesion: 0.10
Nodes (8): RewardLevel, REWARD_POINTS, RewardRequiredPoint, ICreateRewardDto, IRewardGetByIdResult, IUpdateRewardDto, AdminChallengesSystemRewardsComponent, Component

### Community 53 - "RewardsController"
Cohesion: 0.15
Nodes (23): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+15 more)

### Community 54 - "SpaceManagementController"
Cohesion: 0.28
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 55 - "RestApiService"
Cohesion: 0.03
Nodes (39): ITenantListResult, IEventByIdResult, IEventDropdownListResult, GameReviewsService, Injectable, IGameReviewListResult, GetClubInfoModel, SchedulerService (+31 more)

### Community 56 - "AuthenticationService"
Cohesion: 0.11
Nodes (16): DateTime, ApplicationUser, CancellationToken, Task, UserManager, AuthenticationService, TokenResponseModel, Claim (+8 more)

### Community 57 - ".Update"
Cohesion: 0.09
Nodes (23): CancellationToken, Task, ReservationExpirationHandler, CancellationToken, Task, UpdateChallengeCommand, UpdateChallengeCommandHandler, CancellationToken (+15 more)

### Community 58 - "AuthorizedHttpClient"
Cohesion: 0.07
Nodes (24): CancellationToken, HttpMethod, IHttpClientFactory, ILogger, JsonSerializerOptions, string, StringContent, Task (+16 more)

### Community 59 - "DH.Domain.Models.Common"
Cohesion: 0.13
Nodes (8): DH.Application.Common.Queries, DH.Domain.Models.Common, DH.Application.Common.Commands, DH.Adapter.Authentication.Filters, DH.Api.Controllers, DH.Domain.Adapters.Authentication.Enums, ActionAuthorizeAttribute, TypeFilterAttribute

### Community 60 - "DH.Messaging.Publisher"
Cohesion: 0.13
Nodes (13): DH.Messaging.Publisher.Messages, DH.Messaging.HttpClient.Helpers, DH.Messaging.HttpClient, DH.Messaging.HttpClient.Enums, DH.ServiceBusWorker, DH.Statistics.WorkerService.Handlers, DH.Messaging.Publisher, IServiceCollection (+5 more)

### Community 61 - "TenantContextService"
Cohesion: 0.05
Nodes (31): TenantLayoutComponent, Component, ExceptionBaseComponent, ForbiddenComponent, Component, ForbiddenRoutingModule, routes, NgModule (+23 more)

### Community 62 - "EmployeeService"
Cohesion: 0.08
Nodes (21): CancellationToken, ILogger, RoleManager, Task, UserManager, EmployeeService, CreateEmployeePasswordRequest, List (+13 more)

### Community 63 - "OwnerService"
Cohesion: 0.10
Nodes (17): CancellationToken, ILogger, RoleManager, Task, UserManager, OwnerService, PasswordGenerator, CreateOwnerForTenantSetupRequest (+9 more)

### Community 64 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 65 - "rooms.service.ts"
Cohesion: 0.07
Nodes (9): IRoomByIdResult, IRoomListResult, IRoomMemberResult, IRoomMessageResult, AddUpdateMeepleRoomComponent, futureDateValidator(), Component, RoomMembersComponent (+1 more)

### Community 66 - "RoomChatComponent"
Cohesion: 0.15
Nodes (6): IRoomInfoMessageResult, GroupedChatMessage, IGroupMessage, RoomChatComponent, Component, ViewChild

### Community 67 - "DH.Domain.Models.ChallengeModels.Queries"
Cohesion: 0.10
Nodes (5): DH.Domain.Models.ChallengeModels.Queries, DH.Application.Challenges.Qureies, DH.Domain.Models.ChallengeModels.Commands, DH.Application.Challenges.Commands, GetChallengeDropdownListQueryModel

### Community 68 - "ChallengesManagementComponent"
Cohesion: 0.13
Nodes (5): IUserCustomPeriodChallenge, ChallengesManagementComponent, Component, ViewChild, ViewChildren

### Community 69 - "dependencies"
Cohesion: 0.07
Nodes (27): @angular/animations, @angular/compiler, @angular/forms, @angular/material, @angular/platform-browser, @angular/router, @capacitor/android, @capacitor/app (+19 more)

### Community 70 - "devDependencies"
Cohesion: 0.07
Nodes (27): @angular/cli, @angular/compiler-cli, @angular-devkit/build-angular, @capacitor/cli, devDependencies, @angular/cli, @angular/compiler-cli, @angular-devkit/build-angular (+19 more)

### Community 71 - "DH.DiceHub/DH.Domain/DH.Domain.csproj"
Cohesion: 0.09
Nodes (25): DH.Adapter.ChallengesOrchestrator, net8.0, Microsoft.Extensions.Hosting.Abstractions (8.0.0), Microsoft.NET.Sdk, DH.Adapter.ChatHub, net8.0, Microsoft.AspNetCore.SignalR (1.0.4), Microsoft.Extensions.DependencyInjection (8.0.0) (+17 more)

### Community 72 - "EventsLibraryComponent"
Cohesion: 0.20
Nodes (3): IEventListResult, EventsLibraryComponent, Component

### Community 73 - "AddUpdateEventComponent"
Cohesion: 0.06
Nodes (12): IGameCreateDto, IGameUpdateDto, AddUpdateEventComponent, futureDateValidator(), isFutureDate(), parseDateInput(), Component, ViewChild (+4 more)

### Community 74 - "TenantIsolationFixture"
Cohesion: 0.16
Nodes (13): DH.DiceHub.IntegrationTests, int, string, Task, TenantIsolationFixture, Task, TenantIsolationTests, Fact (+5 more)

### Community 75 - "DH.Domain.Queue"
Cohesion: 0.06
Nodes (16): DH.Adapter.GameSession, DH.Domain.Queue, DH.Adapter.ChallengesOrchestrator, DH.Domain.Services.Queue, DH.Domain.Adapters.ChallengesOrchestrator, IServiceCollection, ChallengesOrchestratorAdapterDI, IServiceCollection (+8 more)

### Community 76 - "UniversalChallengeProcessing"
Cohesion: 0.22
Nodes (9): CancellationToken, IDbContextFactory, ILogger, Task, TenantDbContext, UniversalChallengeProcessing, Task, IChallengeHubClient (+1 more)

### Community 77 - "IReservationCleanupQueue"
Cohesion: 0.08
Nodes (26): CancellationToken, Task, CreateGameReservationCommand, CreateGameReservationCommandHandler, CancellationToken, Task, DeclineGameReservationCommand, DeclineGameReservationCommandHandler (+18 more)

### Community 78 - "IGameSessionQueue"
Cohesion: 0.08
Nodes (27): CancellationToken, ILogger, Task, CloseSpaceTableCommand, CloseSpaceTableCommandHandler, CancellationToken, ILogger, Task (+19 more)

### Community 79 - "StatisticController"
Cohesion: 0.24
Nodes (14): CancellationToken, HttpDelete, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticController (+6 more)

### Community 80 - "challenges.service.ts"
Cohesion: 0.21
Nodes (13): ChallengeRewardPoint, ChallengeStatus, IChallengeResult, IChallengeListResult, ICreateChallengeDto, IUniversalChallengeListResult, IUpdateChallengeDto, IUpdateUniversalChallengeDto (+5 more)

### Community 81 - "PushNotificationsService"
Cohesion: 0.16
Nodes (11): CancellationToken, IEnumerable, ILogger, List, Task, PushNotificationsService, Task, INotificationRenderer (+3 more)

### Community 83 - "ChallengesController"
Cohesion: 0.33
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 85 - "IRabbitMqUserContext"
Cohesion: 0.09
Nodes (15): BasicDeliverEventArgs, BasicProperties, DH.Messaging.Publisher.Extensions, DH.Messaging.Publisher.Authentication, IRabbitMqUserContext, IRabbitMqUserContextFactory, RabbitMqUserContext, RabbitMqUserContextFactory (+7 more)

### Community 86 - "ControllerBase"
Cohesion: 0.06
Nodes (53): ActionResult, ControllerBase, DH.OperationResultCore.FrontEndErrors, ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost (+45 more)

### Community 87 - ".UpdateSettings"
Cohesion: 0.15
Nodes (18): ActionAuthorize, AllowAnonymous, CancellationToken, HttpGet, HttpPut, IActionResult, IMediator, ProducesResponseType (+10 more)

### Community 88 - "IValidableFields"
Cohesion: 0.09
Nodes (17): List, ValidationError, CreateChallengeDto, List, ValidationError, UpdateUniversalChallengeDto, DateTime, List (+9 more)

### Community 89 - "AddUpdateClubSpaceComponent"
Cohesion: 0.18
Nodes (3): IAddSpaceTableDto, AddUpdateClubSpaceComponent, Component

### Community 90 - "app.module.ts"
Cohesion: 0.06
Nodes (33): AppModule, NgModule, AppRoutingModule, NgModule, ROUTES, ConfirmEmailModule, NgModule, CreateEmployeePasswordModule (+25 more)

### Community 91 - "SpaceBookingComponent"
Cohesion: 0.09
Nodes (7): DiceRollerComponent, Component, Input, Output, SpaceBookingComponent, Component, ViewChild

### Community 92 - "DH.Domain.Adapters.Scheduling"
Cohesion: 0.13
Nodes (6): DH.Domain.Adapters.Scheduling, DH.Adapter.Scheduling, DH.Adapter.Scheduling.Jobs, DH.Domain.Adapters.Scheduling.Models, DH.Domain.Helpers, TenantSettingsExtensions

### Community 93 - ".AddEmailAdapter"
Cohesion: 0.50
Nodes (3): IConfiguration, IServiceCollection, DI

### Community 94 - "UserManagementService"
Cohesion: 0.10
Nodes (16): CancellationToken, ILogger, List, RoleManager, Task, UserManager, UserManagementService, Role (+8 more)

### Community 95 - "EmailType.cs"
Cohesion: 0.36
Nodes (9): string, EmployeePasswordCreation, ForgotPasswordResetKeys, OwnerPasswordCreation, PartnerInquiryRequest, RegistrationEmailTemplateKeys, TenantApplicationEmailVerification, TenantOwnerCredentials (+1 more)

### Community 96 - ".RefreshAccessTokenAsync"
Cohesion: 0.22
Nodes (5): Claim, IEnumerable, List, Task, RoleHelper

### Community 97 - "QueuedJob"
Cohesion: 0.18
Nodes (10): CancellationToken, IDbContextFactory, ILogger, List, Task, QueuedJobService, QueuedJobSystemUserContext, DateTime (+2 more)

### Community 98 - "TenantSetting"
Cohesion: 0.09
Nodes (18): Task, TenantDbContext, TenantService, Task, TenantRouteValidationMiddleware, Task, ITenantService, DateTime (+10 more)

### Community 99 - "IStatisticJobInfo"
Cohesion: 0.16
Nodes (14): IServiceScopeFactory, StatisticJobFactory, CancellationToken, Task, IStatisticJob, IStatisticJobInfo, IStatisticJobFactory, StatisticJobType (+6 more)

### Community 100 - "HeaderComponent"
Cohesion: 0.12
Nodes (4): HeaderComponent, Component, Input, Output

### Community 101 - ".buildTenantUrl"
Cohesion: 0.05
Nodes (14): initializeUserFactory(), IResetPasswordRequest, RegisterChoiceComponent, Component, AuthRedirectGuard, Injectable, ChallengeUserAccessGuard, Injectable (+6 more)

### Community 102 - "DH.Statistics.Data"
Cohesion: 0.08
Nodes (23): DH.Statistics.Data.Migrations, DH.Statistics.Data, DH.Statistics.Application.Commands, CancellationToken, IDbContextFactory, Task, CreateClubVisitorLogCommand, CreateClubVisitorLogCommandHandler (+15 more)

### Community 103 - "AddUpdateGameComponent"
Cohesion: 0.10
Nodes (6): GameAveragePlaytime, ICreateGameDto, IUpdateGameDto, AddUpdateGameComponent, Component, ViewChild

### Community 104 - "GetSeedGameCatalogDropdownListQueryHandler"
Cohesion: 0.46
Nodes (6): CancellationToken, List, Task, GetSeedGameCatalogDropdownListQuery, GetSeedGameCatalogDropdownListQueryHandler, GetSeedGameCatalogDropdownListQueryModel

### Community 105 - "GetGameReservedListQueryHandler"
Cohesion: 0.31
Nodes (8): CancellationToken, List, Task, GameRecord, GetGameReservedListQuery, GetGameReservedListQueryHandler, DateTime, GetGameReservationListQueryModel

### Community 106 - "AssistiveTouchComponent"
Cohesion: 0.13
Nodes (8): TenantUserSettingsService, Injectable, AssistiveTouchSettings, AssistiveTouchComponent, Component, HostListener, Input, Output

### Community 107 - "ConsoleFileLogger"
Cohesion: 0.11
Nodes (10): DH.Database.MigrationUtility, StreamWriter, bool, ConsoleFileLogger, EnvironmentSettings, Assembly, IServiceCollection, List (+2 more)

### Community 108 - "TenantDbConnectionInterceptor"
Cohesion: 0.14
Nodes (13): DbConnectionInterceptor, CancellationToken, ConnectionEndEventData, DbConnection, IHttpContextAccessor, Task, ApplicationDbConnectionInterceptor, CancellationToken (+5 more)

### Community 109 - ".EnsureRoleAsync"
Cohesion: 0.15
Nodes (12): IConfiguration, IServiceCollection, AuthenticationDIModule, IServiceProvider, RoleManager, Task, UserManager, ApplicationDbContextSeeder (+4 more)

### Community 110 - "DataRepository"
Cohesion: 0.31
Nodes (8): CancellationToken, Expression, Func, IEnumerable, List, Task, TenantDbContext, DataRepository

### Community 111 - "GameService"
Cohesion: 0.17
Nodes (11): IDbContextFactory, Task, GameSeeder, CancellationToken, List, MemoryStream, Task, TenantDbContext (+3 more)

### Community 112 - "SendOwnerCreatePasswordEmailCommandHandler"
Cohesion: 0.10
Nodes (18): ILogger, SmtpEmailSender, CancellationToken, IConfiguration, ILogger, Task, CreatePartnerInquiriesCommand, CreatePartnerInquiriesCommandHandle (+10 more)

### Community 113 - "DataSeeder"
Cohesion: 0.13
Nodes (10): IDbContextFactory, IEnumerable, ILogger, Task, DataSeeder, DataSeederSystemUserContext, Task, IDataSeeder (+2 more)

### Community 114 - "IUniversalChallengeProcessing"
Cohesion: 0.19
Nodes (9): IJobExecutionContext, Task, EventChecker, IJobExecutionContext, Task, UserChallengeTop3StreakTrackerJob, CancellationToken, Task (+1 more)

### Community 115 - "AppIdentityDbContext"
Cohesion: 0.10
Nodes (13): CancellationToken, DbContextOptionsBuilder, ModelBuilder, Task, AppIdentityDbContext, IConfiguration, AppIdentityDbContextFactory, IServiceProvider (+5 more)

### Community 116 - "AuthorizedClientFactory"
Cohesion: 0.29
Nodes (5): IHttpClientFactory, ILogger, AuthorizedClientFactory, ApplicationApi, ApplicationUrlHelper

### Community 117 - "IGameService"
Cohesion: 0.22
Nodes (10): CancellationToken, Task, GetGameByIdQuery, GetGameByIdQueryHandler, GetGameByIdQueryModel, CancellationToken, List, MemoryStream (+2 more)

### Community 118 - "Chart2Component"
Cohesion: 0.20
Nodes (3): Chart2Component, Component, ViewChild

### Community 119 - "IUserContext"
Cohesion: 0.06
Nodes (22): DH.Messaging.HttpClient.UserContext, DH.Statistics.Api.Filters, DH.Statistics.Application, DH.Authentication.UserContext, ActionExecutedContext, ActionExecutingContext, ValidationFilterAttribute, IServiceCollection (+14 more)

### Community 120 - "IRabbitMqClient"
Cohesion: 0.07
Nodes (33): DateTimeOffset, IAuthorizedClientFactory, EventMessage, IRabbitMqClient, CancellationToken, Task, IServiceBusHandler, DateTime (+25 more)

### Community 121 - "StatisticsController"
Cohesion: 0.46
Nodes (7): CancellationToken, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticsController

### Community 122 - "ISeedService"
Cohesion: 0.12
Nodes (11): IMediator, Task, ChallengesSeedService, IMediator, Task, GamesSeedService, IMediator, Task (+3 more)

### Community 123 - "GetGameListQueryModel"
Cohesion: 0.18
Nodes (16): CancellationToken, List, Task, GetGameListByCategoryIdQuery, GetGameListByCategoryIdQueryHandler, CancellationToken, List, Task (+8 more)

### Community 124 - "DH.Authentication.UserContext.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.1), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, net8.0, Swashbuckle.AspNetCore (6.6.2) (+5 more)

### Community 125 - "IQueuedJobService"
Cohesion: 0.20
Nodes (7): Task, StatisticQueuePublisher, CancellationToken, List, Task, IQueuedJobService, IDomainService

### Community 126 - "DH.Statistics.WorkerService.csproj"
Cohesion: 0.12
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.0), Microsoft.Extensions.Logging (8.0.0), Microsoft.NET.Sdk, net8.0, Microsoft.NET.Sdk (+5 more)

### Community 127 - "RabbitMqWorker"
Cohesion: 0.18
Nodes (8): Func, Task, CancellationToken, IServiceProvider, IServiceScope, string, Task, RabbitMqWorker

### Community 128 - "VisitorsChartComponent"
Cohesion: 0.23
Nodes (3): Component, ViewChild, VisitorsChartComponent

### Community 129 - "LoadingIndicatorComponent"
Cohesion: 0.40
Nodes (4): LoadingIndicatorComponent, Component, ContentChild, Input

### Community 130 - ".UploadQrCode"
Cohesion: 0.14
Nodes (12): DH.Domain.Models.ScannerModels.Queries, ActionAuthorize, CancellationToken, HttpPost, IActionResult, ProducesResponseType, Task, ScannerController (+4 more)

### Community 131 - "DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Http.Abstractions (2.3.0), Microsoft.EntityFrameworkCore (8.0.3), Microsoft.EntityFrameworkCore.Design (8.0.3), Microsoft.EntityFrameworkCore.SqlServer (8.0.3), Microsoft.EntityFrameworkCore.Tools (8.0.3), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.2), Microsoft.NET.Sdk (+5 more)

### Community 132 - "DH.Statistics.Application/Queries/GetActivityChartDataQuery.cs"
Cohesion: 0.18
Nodes (12): CancellationToken, DateTime, IDbContextFactory, List, Task, GetActivityChartDataQuery, GetActivityChartDataQueryHandler, ChartActivityType (+4 more)

### Community 133 - "ApiExceptionFilterAttribute"
Cohesion: 0.13
Nodes (9): ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute, ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute (+1 more)

### Community 134 - "TenantApplicationDto"
Cohesion: 0.21
Nodes (12): CancellationToken, Task, GetTenantApplicationByIdQuery, GetTenantApplicationByIdQueryHandler, CancellationToken, List, Task, GetTenantApplicationsQuery (+4 more)

### Community 135 - "DH.Database.MigrationUtility.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2), NodaTime (3.2.2), Microsoft.NET.Sdk, Mapster (7.4.1-pre01), Microsoft.Extensions.Caching.Memory (8.0.1), Microsoft.Extensions.Configuration.Json (8.0.1) (+5 more)

### Community 136 - "GetEventListQueryModel"
Cohesion: 0.14
Nodes (22): CancellationToken, List, Task, GetEventListForStaffQuery, GetEventListForStaffQueryHandler, CancellationToken, List, Task (+14 more)

### Community 137 - "scripts"
Cohesion: 0.13
Nodes (14): name, private, scripts, build, cap:open, cap:sync, ng, prod-build (+6 more)

### Community 138 - "LandingComponent"
Cohesion: 0.15
Nodes (5): PartnerInquiriesService, Injectable, IPartnerInquiryRequest, LandingComponent, Component

### Community 139 - "MeepleRoomMenuComponent"
Cohesion: 0.09
Nodes (8): MeepleRoomDetailsComponent, Component, ViewChild, MeepleRoomMenuComponent, Component, HostListener, Input, Output

### Community 140 - "IRoomService"
Cohesion: 0.24
Nodes (9): CancellationToken, Task, GetRoomByIdQuery, GetRoomByIdQueryHandler, DateTime, GetRoomByIdQueryModel, CancellationToken, Task (+1 more)

### Community 142 - "Tenant Isolation Plan"
Cohesion: 0.13
Nodes (14): 0. Tenant contract, 10. Completion criteria, 1. Reproduce and baseline the leak, 2. Resolve tenant context consistently, 3. Fix database connection isolation, 4. Verify and enforce PostgreSQL RLS, 5. Complete the entity model inventory, 6. Audit queries and caches (+6 more)

### Community 143 - "AppComponent"
Cohesion: 0.18
Nodes (5): AppComponent, Component, ViewChild, app, messaging

### Community 144 - "EventService"
Cohesion: 0.29
Nodes (7): CancellationToken, IDbContextFactory, List, MemoryStream, Task, EventService, UpdateEventResponseModel

### Community 145 - "SchedulerService"
Cohesion: 0.21
Nodes (8): CancellationToken, ILogger, ISchedulerFactory, List, Task, SchedulerService, DateTime, ScheduleJobInfo

### Community 146 - "GetTenantListQueryModel"
Cohesion: 0.23
Nodes (11): CancellationToken, Task, GetTenantByIdQuery, GetTenantByIdQueryHandler, CancellationToken, List, Task, GetTenantListQuery (+3 more)

### Community 147 - "GetSpaceActivityStatsQueryHandler"
Cohesion: 0.36
Nodes (6): CancellationToken, ILogger, Task, GetSpaceActivityStatsQuery, GetSpaceActivityStatsQueryHandler, GetSpaceActivityStatsQueryModel

### Community 148 - "DH.Database.Connector.csproj"
Cohesion: 0.14
Nodes (11): net8.0, Microsoft.EntityFrameworkCore (8.0.11), Microsoft.EntityFrameworkCore.SqlServer (8.0.11), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4), Microsoft.NET.Sdk, net8.0, Microsoft.EntityFrameworkCore.Design (8.0.11), Microsoft.EntityFrameworkCore.Tools (8.0.11) (+3 more)

### Community 149 - ".ChangePassword"
Cohesion: 0.18
Nodes (6): ChangePasswordRequest, LoginRequest, ResetPasswordRequest, CancellationToken, Task, IAuthenticationService

### Community 151 - "SendTenantApplicationEmailVerificationCodeCommandHandler"
Cohesion: 0.24
Nodes (7): CancellationToken, ILogger, IMemoryCache, int, Task, SendTenantApplicationEmailVerificationCodeCommand, SendTenantApplicationEmailVerificationCodeCommandHandler

### Community 152 - "DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs"
Cohesion: 0.23
Nodes (11): CancellationToken, DateTime, DayOfWeek, IDbContextFactory, List, Task, ChallengeHistoryLogType, DateTimeExtensions (+3 more)

### Community 153 - "options"
Cohesion: 0.19
Nodes (14): options, baseHref, browser, index, inlineStyleLanguage, outputPath, polyfills, scripts (+6 more)

### Community 154 - "CreateEmployeePasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateEmployeePasswordRequest, CreateEmployeePasswordComponent, Component

### Community 155 - "CreateOwnerPasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateOwnerPasswordRequest, CreateOwnerPasswordComponent, Component

### Community 156 - "SendTenantSetupInvitationCommandHandler"
Cohesion: 0.24
Nodes (7): CancellationToken, IConfiguration, ILogger, int, Task, SendTenantSetupInvitationCommand, SendTenantSetupInvitationCommandHandler

### Community 158 - "ReservationHistoryActionsComponent"
Cohesion: 0.14
Nodes (5): ReservationHistoryActionsComponent, Component, ContentChild, Input, Output

### Community 160 - "Google Cloud Setup and Deployment Notes"
Cohesion: 0.15
Nodes (12): **1. Create a Google Cloud Project**, **2. Set Up a Virtual Machine (VM) on Google Cloud**, **3. Install .NET Core SDK and Runtime on the VM**, **4. Deploy the Migration Utility to the VM**, **5. Connect to the VM**, a. **Generate a New SSH Key Pair**, **Autofac Version Issue**, b. **Add the New Public Key to Your Google Cloud VM** (+4 more)

### Community 161 - "NotificationsController"
Cohesion: 0.18
Nodes (13): ActionAuthorize, CancellationToken, HttpGet, HttpPost, IActionResult, ProducesResponseType, Task, NotificationsController (+5 more)

### Community 162 - "VerifyTenantApplicationEmailVerificationCodeCommandHandler"
Cohesion: 0.21
Nodes (8): Entry, TenantApplicationEmailVerificationCache, CancellationToken, IMemoryCache, int, Task, VerifyTenantApplicationEmailVerificationCodeCommand, VerifyTenantApplicationEmailVerificationCodeCommandHandler

### Community 163 - ".GetUserLocalOrUtcTime"
Cohesion: 0.18
Nodes (7): DateTime, NotificationRendererExtensions, DateTime, TimeSpan, TimeZoneHelper, IsUtcFallback, LocalTime

### Community 164 - "ReservationType"
Cohesion: 0.24
Nodes (7): ReservationCleanupHelper, CancellationToken, DateTime, List, Task, ReservationCleanupQueue, ReservationType

### Community 165 - "AssistiveTouchComponent"
Cohesion: 0.22
Nodes (5): AssistiveTouchComponent, Component, HostListener, AssistiveTouchModule, NgModule

### Community 166 - "QRCodeContext"
Cohesion: 0.22
Nodes (7): CancellationToken, Exception, IServiceScopeFactory, Task, QRCodeContext, CancellationToken, Task

### Community 167 - "ChallengeType"
Cohesion: 0.23
Nodes (7): AdminChallengesComponent, Component, ChallengeType, ChallengeTypeToggleComponent, Component, Input, Output

### Community 168 - "EventAttendanceByEventsChartComponent"
Cohesion: 0.21
Nodes (3): EventAttendanceByEventsChartComponent, Component, ViewChild

### Community 169 - "GameLayoutComponent"
Cohesion: 0.18
Nodes (5): GameLayoutComponent, Component, Input, Output, NavItemInterface

### Community 170 - "GetRoomListQueryHandler"
Cohesion: 0.29
Nodes (8): CancellationToken, List, Task, GetRoomListQuery, GetRoomListQueryHandler, DateTime, GetRoomListQueryModel, List

### Community 171 - "ISynchronizeUsersChallengesQueue"
Cohesion: 0.32
Nodes (5): CancellationToken, DateTime, List, Task, ISynchronizeUsersChallengesQueue

### Community 172 - "ISpaceTableService"
Cohesion: 0.13
Nodes (18): IJobExecutionContext, Task, CloseActiveTablesJob, CancellationToken, Task, GetActiveSpaceTableReservationCountQuery, GetActiveSpaceTableReservationCountQueryHandler, CancellationToken (+10 more)

### Community 173 - "IDomainService"
Cohesion: 0.13
Nodes (16): CancellationToken, IDbContextFactory, List, Task, GameCategoryService, CancellationToken, List, Task (+8 more)

### Community 174 - "MapPermissions"
Cohesion: 0.18
Nodes (7): UserAction, IUserContext, IActionPermissions, Dictionary, IDictionary, List, MapPermissions

### Community 175 - "GetExpiredCollectedRewardsChartDataQuery"
Cohesion: 0.27
Nodes (8): CancellationToken, IDbContextFactory, Task, GetExpiredCollectedRewardsChartDataQuery, GetExpiredCollectedRewardsChartDataQueryHandler, List, GetExpiredCollectedRewardsChartDataModel, RewardsStats

### Community 176 - "GameReservationHistory"
Cohesion: 0.35
Nodes (3): IGameReservationHistory, GameReservationHistory, Component

### Community 177 - "SpaceTableReservationHistory"
Cohesion: 0.35
Nodes (3): ITableReservationHistory, SpaceTableReservationHistory, Component

### Community 178 - ".resetData"
Cohesion: 0.07
Nodes (7): CollectedExpiredRewardsChartComponent, Component, ViewChild, ClubSpaceListComponent, Component, EventDetailsComponent, Component

### Community 179 - "EventAttendanceChartComponent"
Cohesion: 0.26
Nodes (3): EventAttendanceChartComponent, Component, ViewChild

### Community 180 - "TokenService"
Cohesion: 0.24
Nodes (6): ClaimsPrincipal, DateTime, UserManager, TokenService, TimeSpan, JwtTokenOptions

### Community 181 - ".ValidateQRCodeAsync"
Cohesion: 0.25
Nodes (5): byte, CancellationToken, Task, QrCodeDecryptor, QRCodeManager

### Community 182 - "DH.DiceHub.sln"
Cohesion: 0.18
Nodes (3): DH.Adapter.QRManager, net8.0, Microsoft.NET.Sdk

### Community 183 - "DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj"
Cohesion: 0.18
Nodes (10): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.3), Microsoft.EntityFrameworkCore (8.0.4), Microsoft.EntityFrameworkCore.Design (8.0.4), Microsoft.EntityFrameworkCore.SqlServer (8.0.4), Microsoft.EntityFrameworkCore.Tools (8.0.4), Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4) (+2 more)

### Community 184 - "TenantDbContext"
Cohesion: 0.22
Nodes (6): DH.Database.Connector.Models, DbContext, IDatabaseEntity, Assembly, ModelBuilder, TenantDbContext

### Community 185 - "http"
Cohesion: 0.18
Nodes (10): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, profiles (+2 more)

### Community 186 - "GetCustomPeriodQueryModel"
Cohesion: 0.29
Nodes (9): CancellationToken, Task, GetCustomPeriodQuery, GetCustomPeriodQueryHandler, List, GetCustomPeriodChallengeQueryModel, GetCustomPeriodQueryModel, GetCustomPeriodRewardQueryModel (+1 more)

### Community 188 - "GetActivityChartData"
Cohesion: 0.29
Nodes (8): CancellationToken, Task, GetActivityChartDataQuery, GetActivityChartDataQueryHandler, DateTime, List, ActivityLog, GetActivityChartData

### Community 189 - "GameSessionQueue"
Cohesion: 0.27
Nodes (6): GameSessionHelper, CancellationToken, DateTime, List, Task, GameSessionQueue

### Community 190 - "EmailHelperService"
Cohesion: 0.28
Nodes (5): Dictionary, IDbContextFactory, Task, EmailHelperService, EmailType

### Community 193 - "manifest.json"
Cohesion: 0.18
Nodes (10): background_color, description, display, icons, name, orientation, scope, short_name (+2 more)

### Community 194 - "ActionAuthorizeFilter"
Cohesion: 0.25
Nodes (6): AuthorizationFilterContext, int, Task, ActionAuthorizeFilter, IUserActionService, IAsyncAuthorizationFilter

### Community 195 - ".SubmitInquiry"
Cohesion: 0.15
Nodes (11): CancellationToken, HttpPost, IActionResult, IMediator, Task, PartnerInquiriesController, int, List (+3 more)

### Community 196 - "DH.Adapter.Authentication.Migrations"
Cohesion: 0.29
Nodes (3): DH.Adapter.Authentication.Migrations, ModelBuilder, InitialTenant

### Community 197 - "IAddUserChallengePeriodHandler"
Cohesion: 0.47
Nodes (3): CancellationToken, Task, IAddUserChallengePeriodHandler

### Community 198 - "IJob"
Cohesion: 0.24
Nodes (7): IJobExecutionContext, Task, ExpireReservationJob, CancellationToken, Task, IReservationExpirationHandler, IJob

### Community 199 - "IUserRewardsExpirationReminderHandler"
Cohesion: 0.27
Nodes (6): IJobExecutionContext, Task, UserRewardsExpirationReminderJob, CancellationToken, Task, IUserRewardsExpirationReminderHandler

### Community 200 - "IUserRewardsExpiryHandler"
Cohesion: 0.27
Nodes (6): IJobExecutionContext, Task, UserRewardsExpiryJob, CancellationToken, Task, IUserRewardsExpiryHandler

### Community 201 - "UserSettingsDto"
Cohesion: 0.27
Nodes (8): CancellationToken, Task, GetUserSettingsQuery, GetUserSettingsQueryHandler, bool, List, ValidationError, UserSettingsDto

### Community 202 - "GetGameActivityChartData"
Cohesion: 0.33
Nodes (7): CancellationToken, Task, GetGameActivityChartDataQuery, GetGameActivityChartDataQueryHandler, List, GameActivityStats, GetGameActivityChartData

### Community 203 - "GetUserWhoPlayedGameChartDataQueryHandler"
Cohesion: 0.29
Nodes (8): CancellationToken, Task, GetUserWhoPlayedGameChartDataQuery, GetUserWhoPlayedGameChartDataQueryHandler, DateTime, List, GameUserActivity, GetUsersWhoPlayedGameData

### Community 204 - "RoomService"
Cohesion: 0.39
Nodes (5): CancellationToken, IDbContextFactory, List, Task, RoomService

### Community 205 - ".TryDequeue"
Cohesion: 0.20
Nodes (8): CancellationToken, List, Task, IStatisticJobQueue, CancellationToken, List, Task, StatisticJobQueue

### Community 206 - "QrCodeValidationResult"
Cohesion: 0.31
Nodes (6): CancellationToken, Task, bool, QrCodeType, string, QrCodeValidationResult

### Community 207 - "AuthTokenService"
Cohesion: 0.29
Nodes (4): HttpRequestInterceptor, Injectable, AuthTokenService, Injectable

### Community 208 - "GetGameReviewListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetGameReviewListQuery, GetGameReviewListQueryHandler, DateTime, GetGameReviewListQueryModel

### Community 210 - ".Handle"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomMemberListQuery, GetRoomMemberListQueryHandler, DateTime, GetRoomMemberListQueryModel

### Community 211 - "ReservationsChartComponent"
Cohesion: 0.29
Nodes (3): ReservationsChartComponent, Component, ViewChild

### Community 212 - "DH.Domain.Adapters.Statistics.Services"
Cohesion: 0.09
Nodes (12): DH.Domain.Adapters.Statistics.Services, DH.OperationResultCore, DH.Domain.Adapters.Statistics.JobHandlers, DH.Application.Stats.Queries, DH.Domain.Adapters.Statistics.Enums, DH.OperationResultCore.Utility, DH.Application.Statistics.Queries, DH.Domain.Models.StatisticsModels.Queries (+4 more)

### Community 213 - "ChallengeHubClientProxy"
Cohesion: 0.36
Nodes (3): IHubContext, Task, ChallengeHubClientProxy

### Community 214 - "GetGameReservationHistoryQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetGameReservationHistoryQuery, GetGameReservationHistoryQueryHandler, DateTime, GetGameReservationHistoryQueryModel

### Community 215 - "SpaceTableService"
Cohesion: 0.39
Nodes (5): CancellationToken, IDbContextFactory, List, Task, SpaceTableService

### Community 216 - "DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj"
Cohesion: 0.22
Nodes (8): net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.NET.Sdk, Quartz.AspNetCore (3.13.0), Quartz.Extensions.DependencyInjection (3.13.0), Quartz.Extensions.Hosting (3.13.0), Quartz.Plugins (3.13.0), Quartz.Serialization.Json (3.13.0)

### Community 217 - "GetUserRewardListQueryHandler"
Cohesion: 0.39
Nodes (7): CancellationToken, List, Task, GetUserRewardListQuery, GetUserRewardListQueryHandler, GetUserRewardListQueryModel, UserRewardStatus

### Community 218 - "GetRoomInfoMessageListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomInfoMessageListQuery, GetRoomInfoMessageListQueryHandler, DateTime, GetRoomInfoMessageListQueryModel

### Community 219 - "GetRoomMessageListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomMessageListQuery, GetRoomMessageListQueryHandler, DateTime, GetRoomMessageListQueryModel

### Community 220 - "GetUserActiveTableQueryHandler"
Cohesion: 0.31
Nodes (7): CancellationToken, ILogger, Task, GetUserActiveTableQuery, GetUserActiveTableQueryHandler, DateTime, GetUserActiveTableQueryModel

### Community 221 - "DH.Messaging.Publisher.csproj"
Cohesion: 0.22
Nodes (7): net8.0, Microsoft.Extensions.DependencyInjection (8.0.1), Microsoft.NET.Sdk, net8.0, Microsoft.Extensions.Hosting (8.0.1), Microsoft.NET.Sdk, RabbitMQ.Client (7.0.0)

### Community 222 - "ChallengeProcessingOutcomeMessage"
Cohesion: 0.67
Nodes (3): DateTime, ChallengeOutcome, ChallengeProcessingOutcomeMessage

### Community 223 - "GetReservationChartDataQuery"
Cohesion: 0.39
Nodes (6): CancellationToken, Task, GetReservationChartDataQuery, GetReservationChartDataQueryHandler, GetReservationChartData, ReservationStats

### Community 224 - "development"
Cohesion: 0.22
Nodes (9): build, builder, configurations, defaultConfiguration, development, buildTarget, extractLicenses, optimization (+1 more)

### Community 225 - "production"
Cohesion: 0.22
Nodes (9): serve, production, budgets, buildTarget, fileReplacements, outputHashing, builder, configurations (+1 more)

### Community 226 - "DH.Adapter.FileManager"
Cohesion: 0.29
Nodes (4): DH.Adapter.FileManager, IConfiguration, IServiceCollection, DI

### Community 227 - "GetActiveReservedGameQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetActiveReservedGameQuery, GetActiveReservedGameQueryHandler, DateTime, GetActiveReservedGameQueryModel

### Community 228 - "RewardsCollectedChartComponent"
Cohesion: 0.31
Nodes (3): RewardsCollectedChartComponent, Component, ViewChild

### Community 229 - "ScrollTopComponent"
Cohesion: 0.25
Nodes (5): ScrollTopComponent, Component, HostListener, ScrollToTopModule, NgModule

### Community 230 - "ChatHubClient.cs"
Cohesion: 0.29
Nodes (4): DH.Domain.Adapters.ChatHub, DH.Adapter.ChatHub, IServiceCollection, ChatHubDIModule

### Community 231 - "PermissionStringBuilder"
Cohesion: 0.32
Nodes (5): IMemoryCache, PermissionStringBuilder, IDictionary, List, IMapPermissions

### Community 232 - "AppIdentityDbContextModelSnapshot"
Cohesion: 0.18
Nodes (7): ModelBuilder, AppIdentityDbContextModelSnapshot, ModelBuilder, TenantDbContextModelSnapshot, ModelBuilder, StatisticsDbContextModelSnapshot, ModelSnapshot

### Community 233 - ".JobWasExecuted"
Cohesion: 0.25
Nodes (7): CancellationToken, IJobExecutionContext, IServiceScopeFactory, Task, JobListenerForDeadLetterQueue, JobExecutionException, JobListenerSupport

### Community 234 - "ITenantSettingsCacheService"
Cohesion: 0.11
Nodes (19): IJobExecutionContext, ILogger, ISchedulerFactory, string, Task, AddUserChallengePeriodJob, CancellationToken, List (+11 more)

### Community 235 - "GetAllEventsDropdownListQueryHandler"
Cohesion: 0.50
Nodes (6): CancellationToken, List, Task, GetAllEventsDropdownListModel, GetAllEventsDropdownListQuery, GetAllEventsDropdownListQueryHandler

### Community 236 - "GetActiveGameReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveGameReservationListQuery, GetActiveGameReservationListQueryHandler, DateTime, GetActiveGameReservationListQueryModel

### Community 237 - "GetGameReservationStatusQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetGameReservationStatusQuery, GetGameReservationStatusQueryHandler, DateTime, GetGameReservationStatusQueryModel

### Community 238 - "GetSystemRewardDropdownListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSystemRewardDropdownListQuery, GetSystemRewardDropdownListQueryHandler, GetSystemRewardDropdownListQueryModel

### Community 239 - "GetUserChallengePeriodRewardListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetUserChallengePeriodRewardListQuery, GetUserChallengePeriodRewardListQueryHandler, GetUserChallengePeriodRewardListQueryModel

### Community 240 - "GetActiveSpaceTableReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveSpaceTableReservationListQuery, GetActiveSpaceTableReservationListQueryHandler, DateTime, GetActiveSpaceTableReservationListQueryModel

### Community 241 - "GetSpaceAvailableTableListQuery"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSpaceAvailableTableListQuery, GetSpaceAvailableTableListQueryHandler, GetSpaceAvailableTableListQueryModel

### Community 242 - "GetSpaceTableParticipantListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSpaceTableParticipantListQuery, GetSpaceTableParticipantListQueryHandler, GetSpaceTableParticipantListQueryModel

### Community 243 - "GetGameReservationByIdQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetGameReservationByIdQuery, GetGameReservationByIdQueryHandler, DateTime, GetGameReservationByIdQueryModel

### Community 244 - "SynchronizeUsersChallengesQueue"
Cohesion: 0.32
Nodes (5): CancellationToken, DateTime, List, Task, SynchronizeUsersChallengesQueue

### Community 245 - "DH.WebUI"
Cohesion: 0.25
Nodes (8): prefix, projectType, root, schematics, sourceRoot, DH.WebUI, style, @schematics/angular:component

### Community 246 - "DHWebUI"
Cohesion: 0.25
Nodes (7): Build, Code scaffolding, Development server, DHWebUI, Further help, Running end-to-end tests, Running unit tests

### Community 248 - "ToastComponent"
Cohesion: 0.36
Nodes (3): ToastComponent, Component, Inject

### Community 249 - "SupabaseStorageClient"
Cohesion: 0.43
Nodes (4): Client, IConfiguration, Task, SupabaseStorageClient

### Community 250 - "DH.Statistics.WorkerService.Common"
Cohesion: 0.38
Nodes (4): DH.Statistics.WorkerService.Common, RabbitMqOptions, RabbitMqQueues, RabbitMqRoutingKeys

### Community 251 - "ChallengeReward"
Cohesion: 0.11
Nodes (20): CancellationToken, IDbContextFactory, MemoryStream, Task, RewardService, DateTime, ICollection, ChallengeReward (+12 more)

### Community 252 - "DH.Adapter.Email"
Cohesion: 0.29
Nodes (7): DH.Adapter.Email, net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, Microsoft.Extensions.Options (8.0.2), Microsoft.Extensions.Options.ConfigurationExtensions (8.0.0)

### Community 253 - ".GetGameCategoryList"
Cohesion: 0.29
Nodes (6): ActionAuthorize, CancellationToken, HttpPost, IActionResult, ProducesResponseType, Task

### Community 254 - "GetChallengeListWithFilterQuery"
Cohesion: 0.52
Nodes (6): CancellationToken, List, Task, GetChallengeListWithFilterQuery, GetChallengeListWithFilterQueryHandler, GetChallengeListWithFilterQueryModel

### Community 255 - "GetActiveBookedSpaceTableQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetActiveBookedSpaceTableQuery, GetActiveBookedSpaceTableQueryHandler, DateTime, GetActiveBookedSpaceTableQueryModel

### Community 256 - "GetUserChallengePeriodPerformanceQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetUserChallengePeriodPerformanceQuery, GetUserChallengePeriodPerformanceQueryHandler, DateTime, GetUserChallengePeriodPerformanceQueryModel

### Community 257 - "GetAssistiveTouchSettingsQueryHandler"
Cohesion: 0.43
Nodes (5): CancellationToken, Task, GetAssistiveTouchSettingsQuery, GetAssistiveTouchSettingsQueryHandler, AssistiveTouchSettings

### Community 258 - "GetEventByIdQueryModel"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetEventByIdQuery, GetEventByIdQueryHandler, DateTime, GetEventByIdQueryModel

### Community 259 - "GetGameInventoryQueryHandler"
Cohesion: 0.43
Nodes (5): CancellationToken, Task, GetGameInventoryQuery, GetGameInventoryQueryHandler, GetGameInvetoryQueryModel

### Community 260 - "GetSpaceTableReservationByIdQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetSpaceTableReservationByIdQuery, GetSpaceTableReservationByIdQueryHandler, DateTime, GetSpaceTableReservationByIdQueryModel

### Community 261 - "IGameSessionService"
Cohesion: 0.48
Nodes (3): CancellationToken, Task, IGameSessionService

### Community 262 - "GetChallengeByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetChallengeByIdQuery, GetChallengeByIdQueryHandler, GetChallengeByIdQueryModel

### Community 263 - "DH.DiceHub.IntegrationTests"
Cohesion: 0.29
Nodes (7): DH.DiceHub.IntegrationTests, net8.0, Microsoft.NET.Sdk, Microsoft.NET.Test.Sdk (17.10.0), Npgsql (8.0.3), xunit (2.8.1), xunit.runner.visualstudio (2.8.1)

### Community 264 - "angular.json"
Cohesion: 0.29
Nodes (6): cli, analytics, newProjectRoot, projects, $schema, version

### Community 265 - "architect"
Cohesion: 0.29
Nodes (7): extract-i18n, test, architect, builder, options, buildTarget, builder

### Community 266 - "StreakComponent"
Cohesion: 0.29
Nodes (3): StreakComponent, StreakPageType, Component

### Community 267 - "RandomColorDirective"
Cohesion: 0.33
Nodes (3): RandomColorDirective, Input, Directive

### Community 269 - "DH.Adapter.FileManager"
Cohesion: 0.33
Nodes (6): DH.Adapter.FileManager, net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.NET.Sdk, Supabase (1.1.1), Supabase.Storage (2.4.1)

### Community 270 - ".AddSchedulingAdapter"
Cohesion: 0.47
Nodes (4): IConfiguration, IServiceCollection, SchedulingDIModule, IServiceCollectionQuartzConfigurator

### Community 271 - "DeleteGameCommandHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, DeleteGameCommand, DeleteGameCommandHandler

### Community 272 - "GetActiveGameReservationCountQueryHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, GetActiveGameReservationCountQuery, GetActiveGameReservationCountQueryHandler

### Community 273 - "GetSystemRewardByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetSystemRewardByIdQuery, GetSystemRewardByIdQueryHandler, GetRewardByIdQueryModel

### Community 274 - "DeleteRoomCommandHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, DeleteRoomCommand, DeleteRoomCommandHandler

### Community 275 - "assets"
Cohesion: 0.33
Nodes (6): assets, src/favicon.ico, src/firebase-messaging-sw.js, src/manifest.json, src/shared/assets, src/shared/assets/images

### Community 277 - "UpdateEventModel"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, UpdateEventModel

### Community 278 - "PasswordVisibilityToggleComponent"
Cohesion: 0.33
Nodes (4): PasswordVisibilityToggleComponent, Component, Input, Output

### Community 279 - "ApiEndpoints.cs"
Cohesion: 0.40
Nodes (4): DH.Statistics.WorkerService, string, ApiEndpoints, Statistics

### Community 280 - "Migration"
Cohesion: 0.50
Nodes (3): MigrationBuilder, InitialTenant, Migration

### Community 281 - "DH.Adapter.ChallengeHub"
Cohesion: 0.40
Nodes (5): DH.Adapter.ChallengeHub, net8.0, Microsoft.AspNetCore.SignalR (1.0.4), Microsoft.Extensions.DependencyInjection (8.0.0), Microsoft.NET.Sdk

### Community 282 - ".AddDataAdapter"
Cohesion: 0.60
Nodes (3): IConfiguration, IServiceCollection, DataDIModule

### Community 291 - "DH.Adapter.Localization"
Cohesion: 0.40
Nodes (5): DH.Adapter.Localization, net8.0, Microsoft.NET.Sdk, Microsoft.AspNetCore.Localization (2.3.0), Microsoft.Extensions.Localization (8.0.19)

### Community 292 - "DH.Adapter.PushNotifications"
Cohesion: 0.40
Nodes (5): DH.Adapter.PushNotifications, net8.0, Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, FirebaseAdmin (3.0.1)

### Community 293 - "DH.Adapter.Statistics"
Cohesion: 0.40
Nodes (5): DH.Adapter.Statistics, net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.Extensions.Hosting.Abstractions (8.0.1), Microsoft.NET.Sdk

### Community 294 - ".GetActiveUserCustomPeriod"
Cohesion: 0.60
Nodes (3): ILogger, List, UserChallengePeriodPerformanceExtensions

### Community 295 - "CreateGameReviewDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, CreateGameReviewDto

### Community 296 - "UpdateGameReviewDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, UpdateGameReviewDto

### Community 297 - "CreateRoomCommandDto"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, CreateRoomCommandDto

### Community 298 - "UpdateRoomCommandDto"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, UpdateRoomCommandDto

### Community 301 - "NavBarComponent"
Cohesion: 0.40
Nodes (3): NavBarComponent, Component, Input

### Community 302 - "DH.Adapter.Data.Migrations"
Cohesion: 0.15
Nodes (7): DH.Adapter.Data.Migrations, ModelBuilder, InitialTenant, ModelBuilder, InitialData, ModelBuilder, AddTenantApplications

### Community 303 - "UpdateRewardDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, UpdateRewardDto

### Community 305 - "ChatHubClient"
Cohesion: 0.27
Nodes (5): Task, ChatHubClient, Task, IChatHubClient, Hub

### Community 306 - "ChipComponent"
Cohesion: 0.40
Nodes (3): ChipComponent, Component, Input

### Community 308 - "Models/Common/RabbitMqOptions.cs"
Cohesion: 0.83
Nodes (3): RabbitMqOptions, RabbitMqQueues, RabbitMqRoutingKeys

### Community 310 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, EventQRCodeState

### Community 312 - "gradlew"
Cohesion: 0.83
Nodes (3): gradlew script, die(), warn()

### Community 316 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, UnknownQRCodeState

### Community 360 - "ReservationOutcomeLog"
Cohesion: 0.50
Nodes (3): DateTime, ReservationOutcomeLog, ReservationOutcome

## Knowledge Gaps
- **400 isolated node(s):** `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)`, `xunit.runner.visualstudio (2.8.1)`, `Npgsql (8.0.3)` (+395 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **62 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `DH.Domain.Entities` connect `DH.Domain.Entities` to `TenantSetting`, `DH.Domain.Models.ChallengeModels.Queries`, `DH.Domain.Adapters.Authentication`, `TenantDbContext`, `ChatHubClient.cs`, `DH.OperationResultCore.Exceptions`, `GetAllEventsDropdownListQueryHandler`, `DH.Domain.Queue`, `.GetGlobalTenantSettingsAsync`, `DH.Domain.Adapters.Localization`, `ChallengeReward`, `ISystemUserContextAccessor`, `DH.Domain.Adapters.Statistics.Services`, `UserManagementService`, `DH.Domain.Models.Common`, `DH.Domain.Adapters.Scheduling`, `DH.Domain.Repositories`, `DH.Domain.Adapters.Data`?**
  _High betweenness centrality (0.031) - this node is a cross-community bridge._
- **Why does `IRepository` connect `IRepository` to `GetUserChallengePeriodPerformanceQueryHandler`, `GetAssistiveTouchSettingsQueryHandler`, `GetGameInventoryQueryHandler`, `GetSpaceTableReservationByIdQueryHandler`, `IRequest`, `GetChallengeByIdQueryHandler`, `TenantApplicationDto`, `ILocalizationService`, `IUserContext`, `GetSystemRewardByIdQueryHandler`, `GetTenantListQueryModel`, `GetSpaceActivityStatsQueryHandler`, `IPushNotificationsService`, `SendTenantSetupInvitationCommandHandler`, `ReservationCleanupWorker`, `ChatHubClient`, `ISchedulerService`, `RewardsController`, `AuthenticationService`, `.Update`, `GetCustomPeriodQueryModel`, `OwnerService`, `UserSettingsDto`, `IReservationCleanupQueue`, `IGameSessionQueue`, `GetGameReviewListQueryHandler`, `PushNotificationsService`, `.Handle`, `GetUserRewardListQueryHandler`, `GetRoomInfoMessageListQueryHandler`, `GetRoomMessageListQueryHandler`, `GetUserActiveTableQueryHandler`, `UserManagementService`, `GetActiveReservedGameQueryHandler`, `GetSeedGameCatalogDropdownListQueryHandler`, `GetGameReservedListQueryHandler`, `ITenantSettingsCacheService`, `GetAllEventsDropdownListQueryHandler`, `GetActiveGameReservationListQueryHandler`, `GetGameReservationStatusQueryHandler`, `DataRepository`, `GetSystemRewardDropdownListQueryHandler`, `SendOwnerCreatePasswordEmailCommandHandler`, `GetUserChallengePeriodRewardListQueryHandler`, `GetActiveSpaceTableReservationListQueryHandler`, `GetGameReservationByIdQueryHandler`, `GetSpaceAvailableTableListQuery`, `GetSpaceTableParticipantListQueryHandler`, `GetChallengeListWithFilterQuery`, `GetActiveBookedSpaceTableQueryHandler`?**
  _High betweenness centrality (0.026) - this node is a cross-community bridge._
- **Why does `IUserContext` connect `IUserContext` to `GetUserChallengePeriodPerformanceQueryHandler`, `GetAssistiveTouchSettingsQueryHandler`, `IRequest`, `IRepository`, `IPermissionStringBuilder`, `ILocalizationService`, `EventService`, `SchedulerService`, `ISystemUserContextAccessor`, `IUserManagementService`, `TenantSetupService`, `IChallengeService`, `ReservationCleanupWorker`, `MapPermissions`, `StatisticsService`, `ChatHubClient`, `AuthenticationService`, `.Update`, `EmployeeService`, `OwnerService`, `UserSettingsDto`, `RoomService`, `IReservationCleanupQueue`, `IGameSessionQueue`, `PushNotificationsService`, `ChallengesController`, `SpaceTableService`, `GetUserRewardListQueryHandler`, `GetRoomMessageListQueryHandler`, `GetUserActiveTableQueryHandler`, `UserManagementService`, `QueuedJob`, `GetActiveReservedGameQueryHandler`, `ITenantSettingsCacheService`, `GetGameListQueryModel`, `GetGameReservationStatusQueryHandler`, `GameService`, `SendOwnerCreatePasswordEmailCommandHandler`, `DataSeeder`, `GetUserChallengePeriodRewardListQueryHandler`, `IGameService`, `ChallengeReward`, `GetActiveBookedSpaceTableQueryHandler`?**
  _High betweenness centrality (0.026) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)` to the rest of the system?**
  _400 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `AuthService` be split into smaller, more focused modules?**
  _Cohesion score 0.03703085904920767 - nodes in this community are weakly interconnected._
- **Should `shared.module.ts` be split into smaller, more focused modules?**
  _Cohesion score 0.031029887683228632 - nodes in this community are weakly interconnected._
- **Should `ControlsMenuComponent` be split into smaller, more focused modules?**
  _Cohesion score 0.0907258064516129 - nodes in this community are weakly interconnected._