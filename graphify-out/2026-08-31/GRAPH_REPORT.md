# Graph Report - DiceHub  (2026-08-31)

## Corpus Check
- 1202 files · ~6,060,935 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 7428 nodes · 19170 edges · 380 communities (312 shown, 68 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 576 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `aa65f9e8`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- TenantRouter
- GamesService
- app.module.ts
- LandingComponent
- TenantDbContext
- ITenantDbContext
- DH.Domain.Adapters.Data
- DH.Domain.Entities
- global-settings.component.ts
- DH.Domain.Adapters.Statistics.Services
- DH.Domain.Services
- IValidableFields
- DH.Domain.Adapters.Localization
- TenantApplicationsController
- StatisticsController
- IPushNotificationsService
- auth.service.ts
- .error
- DH.OperationResultCore.Exceptions
- ChallengesController
- DH.Domain.Adapters.Scheduling
- GamesController
- DH.Domain.Models.GameModels.Queries
- ChallengeReward
- DH.Domain.Adapters.Authentication
- .navigateTenant
- RoomsController
- profile.module.ts
- StatisticsService
- MeepleRoomDetailsComponent
- DH.Statistics.Domain.Models.Queries
- instruction-management.module.ts
- .setPreviousUrl
- DH.Statistics.Data
- AdminChallengesCustomPeriodComponent
- IRequestHandler
- AppIdentityDbContext
- ITenantSettingsCacheService
- TenantApplicationsService
- challenges.service.ts
- UserManagementService
- UserController
- ReservationCleanupWorker
- ReservationManagementNavigationComponent
- AuthorizedHttpClient
- ChallengesManagementComponent
- AppComponent
- GameSessionService
- .post
- IGameService
- SpaceManagementController
- EmployeeService
- IStatisticsService
- IRabbitMqUserContext
- IEmailHelperService
- OwnerService
- UserContextFactory
- UserChallengesManagementService
- .RunAsTenantAsync
- NotificationPayload
- AuthenticationService
- GetActiveSpaceTableReservationListQueryHandler
- ChallengesService
- DH.OperationResultCore.Utility
- ApiExceptionFilterAttribute
- http
- .onUpdateReview
- GetSystemRewardByIdQueryHandler
- SupportLanguages
- ChallengeType
- dependencies
- devDependencies
- DH.DiceHub/DH.Domain/DH.Domain.csproj
- TenantSettingsController
- RewardLevel
- RoomChatComponent
- GameLayoutComponent
- IUserContext
- TenantIsolationFixture
- Tenant
- IGameSessionQueue
- IChallengeService
- DH.Database.MigrationUtility
- StatisticController
- NotificationsDialog
- AddUpdateEventComponent
- GameAvailabilityComponent
- DH.Statistics.Api/Filters/ApiExceptionFilterAttribute.cs
- EventDetailsComponent
- IUserChallengesManagementService
- QRCodeManager.cs
- SendRegistrationEmailConfirmationCommandHandler
- SpaceTableActiveReservations
- ControllerBase
- TenantSetupService
- ApplicationDbConnectionInterceptor
- .GetGlobalTenantSettingsAsync
- .put
- ChallengeOverlayComponent
- GameNavigationComponent
- RewardsController
- GetClubInfoQueryHandler
- AddUpdateMeepleRoomComponent
- LinkInfoComponent
- QueuedJob
- CollectedExpiredRewardsChartComponent
- HeaderComponent
- .Handle
- GetGameDropdownListQueryHandler
- SpaceBookingComponent
- ConsoleFileLogger
- ReservationStatus
- .UploadQrCode
- .EnsureRoleAsync
- DataRepository
- DH.Domain.Models.StatisticsModels.Queries
- DataSeeder
- NavigationMenuComponent
- IAuthenticationService
- Chart2Component
- SpaceTableReservationHistory
- QrCodeScannerComponent
- IUniversalChallengeProcessing
- GetAllSystemRewardListQueryHandler
- IEventService
- CompleteTenantSetupCommandHandler
- AddUpdateClubSpaceComponent
- GetEventListQueryModel
- DH.Authentication.UserContext.csproj
- .SubmitInquiry
- DH.Statistics.WorkerService.csproj
- VisitorsChartComponent
- GameReservations
- .RefreshAccessTokenAsync
- DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj
- LoginComponent
- GetGameListQueryModel
- DH.Database.MigrationUtility.csproj
- GetActivityChartData
- scripts
- DH.Domain.Repositories
- ClubSpaceDetailsComponent
- InstallPromptComponent
- Tenant Isolation Plan
- CredentialManagerPlugin
- IFileManagerClient
- AdminEventManagementComponent
- GetTenantListQueryModel
- EventAttendanceByEventsChartComponent
- QRReaderModel
- SchedulerService
- DH.Database.Connector.csproj
- DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs
- options
- CreateOwnerPasswordComponent
- ISystemUserContextAccessor
- EventAttendanceChartComponent
- reservation-management.module.ts
- DH.Adapter.Data.Migrations
- Google Cloud Setup and Deployment Notes
- TenantDirectoryService
- QRCodeContext
- EmailType.cs
- TenantApplicationDto
- .GetUserLocalOrUtcTime
- ReservationType
- AssistiveTouchComponent
- EventService
- .resetData
- MessagingService
- GameReservationHistory
- .get
- DH.Statistics.Domain.Entities
- CreateEmployeePasswordComponent
- TenantDbConnectionInterceptor
- GetGameActivityChartData
- ISchedulerService
- VenueApplicationComponent
- .GetGameCategoryList
- MapPermissions
- AdminEventDetailsComponent
- GetSystemRewardDropdownListQueryHandler
- .ValidateQRCodeAsync
- SendTenantApplicationEmailVerificationCodeCommandHandler
- GameService
- IReservationExpirationHandler
- DH.DiceHub.sln
- InitialTenant
- DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj
- GetExpiredCollectedRewardsChartDataModel
- SendTenantSetupInvitationCommandHandler
- http
- GetCustomPeriodQueryModel
- GameSessionQueue
- GetRoomMessageListQueryHandler
- manifest.json
- GamesChartComponent
- TenantApplicationDetailsComponent
- ReservationsChartComponent
- IUserManagementService
- UserChallengeValidationJob
- .getCurrentLanguage
- AssistiveTouchComponent
- SupabaseStorageClient
- AddUserChallengePeriodHandler
- UserRewardsExpirationReminderJob
- AuthTokenService
- GetUserActiveTableQueryHandler
- UserRewardsExpiryJob
- VerifyTenantApplicationEmailVerificationCodeCommandHandler
- .Handle
- ChallengeHubClientProxy
- ToastService
- DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj
- Run BE + FE in separate terminal windows
- ITenantSettings
- GetUserRewardListQueryHandler
- CreateGameReviewDto
- ActionAuthorizeFilter
- .TryDequeue
- qr-code-scanner.component.ts
- DH.Messaging.Publisher.csproj
- production
- development
- GetRoomInfoMessageListQueryHandler
- UpdateGameReviewDto
- IRequest
- ScrollTopComponent
- UpdateRoomCommandDto
- PermissionStringBuilder
- .JobWasExecuted
- .Update
- GetUserChallengePeriodRewardListQueryHandler
- QrCodeValidationResult
- GetSeedGameCatalogDropdownListQueryHandler
- .GetByAsync
- AdminUniversalChallengesComponent
- SpaceTableService
- GetGameReviewListQueryHandler
- GetGameReservedListQueryHandler
- Deploy DH.Api (deploy.sh)
- ISynchronizeUsersChallengesQueue
- IQueuedJobService
- DH.WebUI
- DHWebUI
- LoadingIndicatorComponent
- .HandleAsync
- CanComponentDeactivate
- ToastComponent
- ServerErrorComponent
- DH.Statistics.WorkerService.Common
- PasswordVisibilityToggleComponent
- DH.Adapter.Email
- SynchronizeUsersChallengesQueue
- ParseDateTagPipe
- AddTenantSettingTimeZoneId
- GetGameInventoryQueryHandler
- IGameSessionService
- DH.DiceHub.IntegrationTests
- angular.json
- architect
- assets
- IChatHubClient
- RandomColorDirective
- OperationResult
- .Handle
- DH.Adapter.FileManager
- challenges-management.module.ts
- GetUserWhoPlayedGameChartDataQueryHandler
- AppIdentityDbContextModelSnapshot.cs
- 20260729093650_AddSeedGameCatalog.Designer.cs
- GetActiveGameReservationListQueryHandler
- GetAllEventsDropdownListQueryHandler
- GetActiveSpaceTableReservationCountQueryHandler
- games-library.component.ts
- ApiEndpoints.cs
- DH.Adapter.ChallengeHub
- .AddDataAdapter
- Migration
- InitialTenant
- InitialData
- AddTenantApplications
- AddSeedGameCatalog
- AddTenantSetupTokens
- FixSeedGameCatalogCategories
- AddTenantApplicationLink
- FixTenantSetupInvitationWording
- DH.Adapter.Localization
- DH.Adapter.PushNotifications
- DH.Adapter.Statistics
- club-space-management.module.ts
- 20260118090517_InitialData.Designer.cs
- GetGameByIdQueryHandler
- ErrorInterceptor
- GetGameReservationHistoryQueryHandler
- DiceRollerComponent
- .GetActiveUserCustomPeriod
- GetSpaceActivityStatsQueryHandler
- GetSpaceAvailableTableListQuery
- RegisterComponent
- RegisterChoiceComponent
- Models/Common/RabbitMqOptions.cs
- GetChallengeListWithFilterQuery
- GetUniversalChallengeListQueryHandler
- GetUserChallengePeriodPerformanceQueryHandler
- ExampleInstrumentedTest
- gradlew
- GetActiveReservedGameQueryHandler
- CalculateRemainingDaysPipe
- MainActivity.java
- GameComplexDataQuery.cs
- DH.Domain.Adapters.Email.Models
- GetGameReservationStatusQueryHandler
- GetGameReservationByIdQueryHandler
- ExampleUnitTest
- DiceHub Design Mockups
- AGENTS.md
- 20260729094735_AddTenantSetupTokens.Designer.cs
- GetActiveBookedSpaceTableQueryHandler
- @angular/fire
- angularx-qrcode
- @auth0/angular-jwt
- GetSpaceTableByIdQueryHandler
- @capacitor-firebase/messaging
- GetSpaceTableReservationByIdQueryHandler
- CLAUDE.md
- crypto-js
- DH.DiceHub/deploy.sh
- 20260831071120_AddTenantSettingTimeZoneId.Designer.cs
- capacitor.config.ts
- DH.WebUI/deploy.sh
- GetSpaceTableParticipantListQueryHandler
- memoize-one
- @microsoft/signalr
- .getClubName
- tslib
- rxjs
- ScanConfirmDialogComponent
- IPermissionStringBuilder
- challenge-dropdown.model.ts
- game-qr-code.model.ts
- tenant-settings.interface.ts
- environment.prod.ts
- README.md
- .AddSchedulingAdapter
- GetChallengeByIdQueryHandler
- ValidateTenantSetupTokenQueryHandler
- DeleteGameCommandHandler
- GetActiveGameReservationCountQueryHandler
- DH.Api/Program.cs
- @angular/common
- GameAveragePlaytime
- NavBarComponent
- @angular/platform-browser
- @capacitor/app
- chartjs-adapter-date-fns
- date-fns
- firebase
- @ngx-translate/http-loader
- CreateRoomCommandDto
- UserSettingsDto
- LocalizationService
- .HandleAsync
- TokenService
- @angular/compiler
- @angular/platform-browser-dynamic

## God Nodes (most connected - your core abstractions)
1. `DH.Domain.Entities` - 224 edges
2. `TenantRouter` - 165 edges
3. `DH.Domain.Enums` - 127 edges
4. `DH.Domain.Repositories` - 118 edges
5. `ToastService` - 118 edges
6. `IRepository` - 115 edges
7. `AuthService` - 110 edges
8. `DH.OperationResultCore.Exceptions` - 108 edges
9. `DH.Domain.Adapters.Localization` - 101 edges
10. `ILocalizationService` - 100 edges

## Surprising Connections (you probably didn't know these)
- `DataSeederSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/DataSeeder.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `QueuedJobSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/Services/QueuedJobService.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `TenantSetupSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Adapter.Data/Services/TenantSetupService.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `TenantOwnerCredentialsSystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Application/Common/Commands/SendTenantOwnerCredentialsEmailCommand.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs
- `EmailHistorySystemUserContext` --implements--> `IUserContext`  [EXTRACTED]
  DH.DiceHub/DH.Application/Emails/Commands/SendEmployeeCreatePasswordEmailCommand.cs → DH.DiceHub/DH.Domain/Adapters/Authentication/IUserContext.cs

## Import Cycles
- None detected.

## Communities (380 total, 68 thin omitted)

### Community 0 - "TenantRouter"
Cohesion: 0.04
Nodes (30): AuthService, Injectable, EventsService, Injectable, Injectable, UsersService, AddUpdateEmployeeComponent, Component (+22 more)

### Community 1 - "GamesService"
Cohesion: 0.04
Nodes (50): GamesService, Injectable, GameAveragePlaytime, ActiveReservedGame, ICreateGameDto, ICreateGameReservation, IGameByIdResult, IGameInventory (+42 more)

### Community 2 - "app.module.ts"
Cohesion: 0.07
Nodes (35): AppModule, initializeUserFactory(), NgModule, AppRoutingModule, NgModule, ROUTES, ConfirmEmailModule, NgModule (+27 more)

### Community 3 - "LandingComponent"
Cohesion: 0.15
Nodes (5): PartnerInquiriesService, Injectable, IPartnerInquiryRequest, LandingComponent, Component

### Community 4 - "TenantDbContext"
Cohesion: 0.03
Nodes (68): CancellationToken, DbContextOptionsBuilder, DbSet, IHttpContextAccessor, ModelBuilder, Task, TenantDbContext, IConfiguration (+60 more)

### Community 5 - "ITenantDbContext"
Cohesion: 0.09
Nodes (20): CancellationToken, Task, DeleteGameReviewByIdCommand, DeleteGameReviewByIdCommandHandler, CancellationToken, Task, DislikeGameCommand, DislikeGameCommandHandler (+12 more)

### Community 6 - "DH.Domain.Adapters.Data"
Cohesion: 0.05
Nodes (24): DH.Adapter.Data.Repositories, DH.Application.Games.Seeders, DH.Application.Games.Commands.Games, DH.Adapter.Data.Seeder, DH.Domain.Models.GameModels.Commands, DH.Domain.Services.Seed, DH.Domain.Seeder, DH.Application.Challenges.Seeders (+16 more)

### Community 7 - "DH.Domain.Entities"
Cohesion: 0.05
Nodes (15): DH.Domain.Models.ChallengeModels.Queries, DH.Application.Challenges.Qureies, DH.Domain.Models.ChallengeModels.Commands, DH.Domain.Entities, DH.Domain.Adapters.Reservations, DH.Application.Challenges.Commands, DH.Domain.Enums, DH.Application.Games.Commands (+7 more)

### Community 8 - "global-settings.component.ts"
Cohesion: 0.08
Nodes (15): TenantUserSettingsService, Injectable, ToggleState, IUserSettings, GlobalSettingsComponent, ITenantSettingsForm, Component, IUserSettingsForm (+7 more)

### Community 9 - "DH.Domain.Adapters.Statistics.Services"
Cohesion: 0.05
Nodes (22): DH.Domain.Adapters.Statistics.Services, DH.Application.Stats.Queries, DH.Domain.Adapters.Statistics, DH.Domain.Queue, DH.Adapter.Statistics, DH.Adapter.ChallengesOrchestrator, DH.Domain.Adapters.Statistics.Enums, DH.Domain.Services.Queue (+14 more)

### Community 10 - "DH.Domain.Services"
Cohesion: 0.06
Nodes (16): DH.Application.Common.Commands, DH.Adapter.Data.Services, DH.Adapter.Email, DH.Domain.Models.EventModels.Queries, DH.Domain.Adapters.FileManager, DH.Domain.Adapters.Email, DH.Application.Events.Queries, DH.Application.Rooms.Queries (+8 more)

### Community 11 - "IValidableFields"
Cohesion: 0.10
Nodes (15): DateTime, List, ValidationError, CreateEventModel, DateTime, List, ValidationError, UpdateEventModel (+7 more)

### Community 12 - "DH.Domain.Adapters.Localization"
Cohesion: 0.08
Nodes (14): DH.Adapter.ChallengeHub, DH.Domain.Adapters.PushNotifications.Messages.Models, DH.Domain.Adapters.PushNotifications, DH.Domain.Adapters.Localization, DH.Domain.Adapters.ChallengeHub, DH.Domain.Models, DH.Domain.Adapters.PushNotifications.Messages.Common, DH.Domain.Adapters.PushNotifications.Messages (+6 more)

### Community 13 - "TenantApplicationsController"
Cohesion: 0.17
Nodes (21): ActionAuthorize, AllowAnonymous, Authorize, CancellationToken, HttpGet, HttpPost, IActionResult, IFormFile (+13 more)

### Community 14 - "StatisticsController"
Cohesion: 0.46
Nodes (7): CancellationToken, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticsController

### Community 15 - "IPushNotificationsService"
Cohesion: 0.05
Nodes (40): ConcurrentDictionary, Exception, IHubContext, Task, ChallengeHubClient, CancellationToken, Task, UserRewardsExpirationReminderHandler (+32 more)

### Community 16 - "auth.service.ts"
Cohesion: 0.03
Nodes (68): TODO: Check this tread…, UserRole, IChangePasswordRequest, IRegisterRequest, IRegisterResponse, ITokenResponse, IUserInfo, ITenantListResult (+60 more)

### Community 17 - ".error"
Cohesion: 0.07
Nodes (3): IUpdateEventDto, ScanResultAdminDialog, Component

### Community 18 - "DH.OperationResultCore.Exceptions"
Cohesion: 0.05
Nodes (20): DH.Domain.Models.RoomModels.Commands, DH.OperationResultCore.Exceptions, DH.Domain.Models.RewardModels.Commands, DH.Adapter.Scheduling.Jobs, DH.Adapter.GameSession, DH.Application.Rewards.Commands, DH.Application.SpaceManagement.Commands, DH.Domain.Adapters.GameSession (+12 more)

### Community 19 - "ChallengesController"
Cohesion: 0.14
Nodes (21): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+13 more)

### Community 20 - "DH.Domain.Adapters.Scheduling"
Cohesion: 0.16
Nodes (7): DH.Domain.Adapters.Scheduling, DH.Domain.Adapters.Scheduling.Models, DH.Domain.Helpers, DH.Adapter.Scheduling.Handlers, DH.Domain.Adapters.Scheduling.Enums, JobType, TenantSettingsExtensions

### Community 21 - "GamesController"
Cohesion: 0.26
Nodes (12): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+4 more)

### Community 22 - "DH.Domain.Models.GameModels.Queries"
Cohesion: 0.09
Nodes (8): DH.Adapter.Authentication.Filters, DH.Api.Controllers, DH.Domain.Models.GameModels.Queries, DH.Application.Games.Queries, DH.Domain.Adapters.Authentication.Enums, DH.Application.Games.Queries.Games, ActionAuthorizeAttribute, TypeFilterAttribute

### Community 23 - "ChallengeReward"
Cohesion: 0.11
Nodes (17): DateTime, ICollection, ChallengeReward, CustomPeriodReward, RewardLevel, int, List, ValidationError (+9 more)

### Community 24 - "DH.Domain.Adapters.Authentication"
Cohesion: 0.10
Nodes (13): DH.Adapter.Authentication.Helper, DH.Domain.Adapters.Authentication.Options, DH.Adapter.Authentication, DH.Domain.Adapters.Authentication.Interfaces, DH.Adapter.Authentication.Entities, DH.Domain.Adapters.Authentication, DH.Domain.Adapters.Authentication.Models, DH.Adapter.Authentication.Services (+5 more)

### Community 25 - ".navigateTenant"
Cohesion: 0.06
Nodes (6): EventsChartsLayoutComponent, Component, RewardChartsLayoutComponent, Component, ProfileComponent, Component

### Community 26 - "RoomsController"
Cohesion: 0.08
Nodes (37): CancellationToken, IDbContextFactory, List, Task, RoomService, ActionAuthorize, CancellationToken, HttpDelete (+29 more)

### Community 27 - "profile.module.ts"
Cohesion: 0.05
Nodes (18): GetClubInfoModel, GetOwnerStats, GetUserStats, IOwnerResult, IUser, ChangePasswordComponent, Component, ClubInfo (+10 more)

### Community 28 - "StatisticsService"
Cohesion: 0.06
Nodes (30): CancellationToken, DateTime, IDbContextFactory, List, Task, StatisticsService, Test, StatisticJobFactory (+22 more)

### Community 29 - "MeepleRoomDetailsComponent"
Cohesion: 0.18
Nodes (3): MeepleRoomDetailsComponent, Component, ViewChild

### Community 30 - "DH.Statistics.Domain.Models.Queries"
Cohesion: 0.06
Nodes (38): DH.Statistics.Application.Queries, DH.Statistics.Api.Controllers, DH.Statistics.Domain.Models.Queries, CancellationToken, IDbContextFactory, List, Task, GetCollectedRewardsByDatesQuery (+30 more)

### Community 31 - "instruction-management.module.ts"
Cohesion: 0.12
Nodes (14): INSTRUCTION_LINK_MAPPINGS, InstructionSection, InstructionTopic, LinkInfoType, StepActionLink, InstructionComponent, Component, InstructionLinksComponent (+6 more)

### Community 32 - ".setPreviousUrl"
Cohesion: 0.08
Nodes (6): EmployeeListComponent, Component, FindMeepleManagementComponent, Component, InstructionManagementComponent, Component

### Community 33 - "DH.Statistics.Data"
Cohesion: 0.06
Nodes (22): DH.Database.Connector.Models, DH.Statistics.Data.Migrations, DH.Statistics.Data, DH.Database.Connector, DbContext, Assembly, IConfiguration, IServiceCollection (+14 more)

### Community 34 - "AdminChallengesCustomPeriodComponent"
Cohesion: 0.07
Nodes (4): IUniversalChallengeDropdownResult, AdminChallengesCustomPeriodComponent, customPeriodValidator(), Component

### Community 35 - "IRequestHandler"
Cohesion: 0.07
Nodes (60): UserContext, GameQRCodeState, GameReservationQRCodeState, PurchaseChallengeQRCodeState, RewardQRCodeState, TableReservationQRCodeState, UpdateUniversalChallengeCommandHandler, CancellationToken (+52 more)

### Community 36 - "AppIdentityDbContext"
Cohesion: 0.11
Nodes (12): CancellationToken, DbContextOptionsBuilder, ModelBuilder, Task, AppIdentityDbContext, IConfiguration, AppIdentityDbContextFactory, IServiceProvider (+4 more)

### Community 37 - "ITenantSettingsCacheService"
Cohesion: 0.10
Nodes (18): CancellationToken, Task, ExpiredRewardInfo, UserRewardsExpiryHandler, CancellationToken, Task, ClubNameResult, GetClubNameQuery (+10 more)

### Community 38 - "TenantApplicationsService"
Cohesion: 0.07
Nodes (19): TenantApplicationsService, Injectable, ICompleteTenantSetupRequest, ICompleteTenantSetupResult, ISeedGameCatalogDropdown, ITenantApplication, ITenantApplicationRequest, ITenantApplicationReviewRequest (+11 more)

### Community 39 - "challenges.service.ts"
Cohesion: 0.22
Nodes (12): ChallengeRewardPoint, ChallengeStatus, IChallengeResult, IChallengeListResult, ICreateChallengeDto, IUniversalChallengeListResult, IUpdateChallengeDto, IUpdateUniversalChallengeDto (+4 more)

### Community 40 - "UserManagementService"
Cohesion: 0.12
Nodes (14): CancellationToken, Dictionary, ILogger, List, RoleManager, Task, UserManager, UserManagementService (+6 more)

### Community 41 - "UserController"
Cohesion: 0.18
Nodes (18): ActionAuthorize, AllowAnonymous, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut (+10 more)

### Community 42 - "ReservationCleanupWorker"
Cohesion: 0.07
Nodes (29): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, SynchronizeUsersChallengesWorker, CancellationToken, ILogger (+21 more)

### Community 43 - "ReservationManagementNavigationComponent"
Cohesion: 0.08
Nodes (4): IResetPasswordRequest, ReservationManagementNavigationComponent, Component, ViewChild

### Community 44 - "AuthorizedHttpClient"
Cohesion: 0.07
Nodes (24): CancellationToken, HttpMethod, IHttpClientFactory, ILogger, JsonSerializerOptions, string, StringContent, Task (+16 more)

### Community 45 - "ChallengesManagementComponent"
Cohesion: 0.12
Nodes (6): IUserCustomPeriodChallenge, IUserCustomPeriodReward, ChallengesManagementComponent, Component, ViewChild, ViewChildren

### Community 46 - "AppComponent"
Cohesion: 0.16
Nodes (5): AppComponent, Component, ViewChild, app, messaging

### Community 47 - "GameSessionService"
Cohesion: 0.14
Nodes (21): completedChallenge, completedUniversalChallenges, CancellationToken, IDbContextFactory, IDbContextTransaction, IEnumerable, ILogger, List (+13 more)

### Community 48 - ".post"
Cohesion: 0.06
Nodes (35): StatisticsService, Injectable, ChallengeLeaderboardType, ChartActivityType, GamesActivityType, ActivityLog, GetActivityChartData, IChallengeLeaderboard (+27 more)

### Community 49 - "IGameService"
Cohesion: 0.34
Nodes (5): CancellationToken, List, MemoryStream, Task, IGameService

### Community 50 - "SpaceManagementController"
Cohesion: 0.29
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 51 - "EmployeeService"
Cohesion: 0.08
Nodes (21): CancellationToken, ILogger, RoleManager, Task, UserManager, EmployeeService, CreateEmployeePasswordRequest, List (+13 more)

### Community 52 - "IStatisticsService"
Cohesion: 0.04
Nodes (46): DH.Domain.Adapters.Statistics.JobHandlers, CancellationToken, Task, GetOwnerStatsQuery, GetOwnerStatsQueryHandler, CancellationToken, Task, GetUserStatsQuery (+38 more)

### Community 53 - "IRabbitMqUserContext"
Cohesion: 0.08
Nodes (17): BasicDeliverEventArgs, BasicProperties, DH.Messaging.Publisher.Extensions, DH.Messaging.Publisher.Authentication, IRabbitMqUserContext, IRabbitMqUserContextFactory, RabbitMqUserContext, RabbitMqUserContextFactory (+9 more)

### Community 54 - "IEmailHelperService"
Cohesion: 0.07
Nodes (30): Dictionary, IDbContextFactory, Task, EmailHelperService, CancellationToken, IConfiguration, ILogger, Task (+22 more)

### Community 55 - "OwnerService"
Cohesion: 0.10
Nodes (17): CancellationToken, ILogger, RoleManager, Task, UserManager, OwnerService, PasswordGenerator, CreateOwnerForTenantSetupRequest (+9 more)

### Community 56 - "UserContextFactory"
Cohesion: 0.15
Nodes (8): IHttpContextAccessor, Task, UserContextFactory, IMemoryCache, Task, UserSettingsCache, Task, IUserSettingsCache

### Community 57 - "UserChallengesManagementService"
Cohesion: 0.10
Nodes (25): DbUpdateException, CancellationToken, IDbContextFactory, IDbContextTransaction, ILogger, List, Task, TenantDbContext (+17 more)

### Community 58 - ".RunAsTenantAsync"
Cohesion: 0.15
Nodes (11): Func, Task, TenantContextScopeRunner, Task, ChatHubClient, Func, Task, ITenantContextScopeRunner (+3 more)

### Community 59 - "NotificationPayload"
Cohesion: 0.03
Nodes (47): ChallengeCompletedNotification, ChallengeUpdatedNotification, RenderableNotification, DateTime, EventDeletedNotification, DateTime, EventReminderNotification, DateTime (+39 more)

### Community 60 - "AuthenticationService"
Cohesion: 0.11
Nodes (16): DateTime, ApplicationUser, CancellationToken, Task, UserManager, AuthenticationService, TokenResponseModel, Claim (+8 more)

### Community 61 - "GetActiveSpaceTableReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveSpaceTableReservationListQuery, GetActiveSpaceTableReservationListQueryHandler, DateTime, GetActiveSpaceTableReservationListQueryModel

### Community 62 - "ChallengesService"
Cohesion: 0.10
Nodes (5): ChallengesService, Injectable, ICustomPeriod, AdminChallengesListComponent, Component

### Community 63 - "DH.OperationResultCore.Utility"
Cohesion: 0.04
Nodes (63): DH.Messaging.Publisher.Messages, DH.Messaging.HttpClient.Helpers, DH.Messaging.HttpClient, DH.Messaging.HttpClient.Enums, DH.ServiceBusWorker, DH.OperationResultCore.Utility, DH.Statistics.WorkerService.Handlers, DH.Messaging.Publisher (+55 more)

### Community 64 - "ApiExceptionFilterAttribute"
Cohesion: 0.13
Nodes (9): ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute, ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute (+1 more)

### Community 65 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 67 - "GetSystemRewardByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetSystemRewardByIdQuery, GetSystemRewardByIdQueryHandler, GetRewardByIdQueryModel

### Community 68 - "SupportLanguages"
Cohesion: 0.05
Nodes (42): SupportLanguages, ICreateEventDto, IEventByIdResult, IEventDropdownListResult, UserRewardStatus, RoomsService, Injectable, IRoomByIdResult (+34 more)

### Community 69 - "ChallengeType"
Cohesion: 0.23
Nodes (7): AdminChallengesComponent, Component, ChallengeType, ChallengeTypeToggleComponent, Component, Input, Output

### Community 70 - "dependencies"
Cohesion: 0.07
Nodes (29): @angular/animations, @angular/core, @angular/forms, @angular/material, @angular/router, @capacitor/android, @capacitor/camera, @capacitor/core (+21 more)

### Community 71 - "devDependencies"
Cohesion: 0.07
Nodes (29): @angular/cli, @angular/compiler-cli, @angular-devkit/build-angular, @capacitor/cli, devDependencies, @angular/cli, @angular/compiler-cli, @angular-devkit/build-angular (+21 more)

### Community 72 - "DH.DiceHub/DH.Domain/DH.Domain.csproj"
Cohesion: 0.09
Nodes (25): DH.Adapter.ChallengesOrchestrator, net8.0, Microsoft.Extensions.Hosting.Abstractions (8.0.0), Microsoft.NET.Sdk, DH.Adapter.ChatHub, net8.0, Microsoft.AspNetCore.SignalR (1.0.4), Microsoft.Extensions.DependencyInjection (8.0.0) (+17 more)

### Community 73 - "TenantSettingsController"
Cohesion: 0.11
Nodes (23): ActionAuthorize, AllowAnonymous, CancellationToken, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+15 more)

### Community 74 - "RewardLevel"
Cohesion: 0.42
Nodes (6): RewardLevel, REWARD_POINTS, RewardRequiredPoint, ICreateRewardDto, IRewardGetByIdResult, IUpdateRewardDto

### Community 75 - "RoomChatComponent"
Cohesion: 0.15
Nodes (6): IRoomInfoMessageResult, GroupedChatMessage, IGroupMessage, RoomChatComponent, Component, ViewChild

### Community 76 - "GameLayoutComponent"
Cohesion: 0.18
Nodes (5): GameLayoutComponent, Component, Input, Output, NavItemInterface

### Community 77 - "IUserContext"
Cohesion: 0.06
Nodes (21): DH.Messaging.HttpClient.UserContext, DH.Statistics.Api.Filters, DH.Authentication.UserContext, ActionExecutedContext, ActionExecutingContext, ValidationFilterAttribute, IServiceCollection, DI (+13 more)

### Community 78 - "TenantIsolationFixture"
Cohesion: 0.16
Nodes (13): DH.DiceHub.IntegrationTests, int, string, Task, TenantIsolationFixture, Task, TenantIsolationTests, Fact (+5 more)

### Community 79 - "Tenant"
Cohesion: 0.09
Nodes (18): IMemoryCache, Task, TenantDbContext, TimeSpan, TenantService, HttpContext, Task, TenantRouteValidationMiddleware (+10 more)

### Community 80 - "IGameSessionQueue"
Cohesion: 0.11
Nodes (17): ILogger, CloseSpaceTableCommand, CloseSpaceTableCommandHandler, CancellationToken, ILogger, Task, LeaveSpaceTableCommand, LeaveSpaceTableCommandHandler (+9 more)

### Community 81 - "IChallengeService"
Cohesion: 0.06
Nodes (45): CancellationToken, IDbContextFactory, List, Task, ChallengeService, CancellationToken, Task, DeleteChallengeCommand (+37 more)

### Community 83 - "StatisticController"
Cohesion: 0.24
Nodes (14): CancellationToken, HttpDelete, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticController (+6 more)

### Community 84 - "NotificationsDialog"
Cohesion: 0.10
Nodes (7): NotificationsService, Injectable, IUserNotification, NotificationsDialog, Component, Inject, ViewChild

### Community 85 - "AddUpdateEventComponent"
Cohesion: 0.13
Nodes (6): AddUpdateEventComponent, futureDateValidator(), isFutureDate(), parseDateInput(), Component, ViewChild

### Community 87 - "DH.Statistics.Api/Filters/ApiExceptionFilterAttribute.cs"
Cohesion: 0.40
Nodes (3): DH.OperationResultCore, Dictionary, IError

### Community 89 - "IUserChallengesManagementService"
Cohesion: 0.46
Nodes (3): CancellationToken, Task, IUserChallengesManagementService

### Community 90 - "QRCodeManager.cs"
Cohesion: 0.17
Nodes (7): DH.Domain.Adapters.QRManager.StateModels, DH.Adapter.QRManager, DH.Adapter.QRManager.QRCodeStates, DH.Domain.Adapters.QRManager, IServiceCollection, QRManagerDIModule, QrCodeType

### Community 91 - "SendRegistrationEmailConfirmationCommandHandler"
Cohesion: 0.10
Nodes (18): ILogger, SmtpEmailSender, CancellationToken, IConfiguration, ILogger, Task, CreatePartnerInquiriesCommand, CreatePartnerInquiriesCommandHandle (+10 more)

### Community 93 - "ControllerBase"
Cohesion: 0.05
Nodes (61): ActionResult, ControllerBase, DH.OperationResultCore.FrontEndErrors, ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost (+53 more)

### Community 94 - "TenantSetupService"
Cohesion: 0.21
Nodes (10): CancellationToken, List, Task, TenantDbContext, TenantSetupService, TenantSetupSystemUserContext, DateTime, SeedGameCatalog (+2 more)

### Community 95 - "ApplicationDbConnectionInterceptor"
Cohesion: 0.25
Nodes (8): DbConnectionInterceptor, CancellationToken, ConnectionEndEventData, DbConnection, HttpContext, IHttpContextAccessor, Task, ApplicationDbConnectionInterceptor

### Community 96 - ".GetGlobalTenantSettingsAsync"
Cohesion: 0.15
Nodes (14): CancellationToken, IDbContextFactory, ILogger, Task, TenantDbContext, UniversalChallengeProcessing, Task, IChallengeHubClient (+6 more)

### Community 98 - "ChallengeOverlayComponent"
Cohesion: 0.14
Nodes (4): ChallengeHubService, Injectable, ChallengeOverlayComponent, Component

### Community 99 - "GameNavigationComponent"
Cohesion: 0.11
Nodes (6): GameCategoriesComponent, Component, GameNavigationComponent, Component, NewGameListComponent, Component

### Community 100 - "RewardsController"
Cohesion: 0.32
Nodes (12): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+4 more)

### Community 101 - "GetClubInfoQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetClubInfoModel, GetClubInfoQuery, GetClubInfoQueryHandler

### Community 102 - "AddUpdateMeepleRoomComponent"
Cohesion: 0.12
Nodes (4): IAddUpdateRoomDto, AddUpdateMeepleRoomComponent, futureDateValidator(), Component

### Community 103 - "LinkInfoComponent"
Cohesion: 0.16
Nodes (6): InstructionStep, LinkInfoComponent, Component, HostListener, Input, ViewChild

### Community 104 - "QueuedJob"
Cohesion: 0.18
Nodes (10): CancellationToken, IDbContextFactory, ILogger, List, Task, QueuedJobService, QueuedJobSystemUserContext, DateTime (+2 more)

### Community 105 - "CollectedExpiredRewardsChartComponent"
Cohesion: 0.25
Nodes (3): CollectedExpiredRewardsChartComponent, Component, ViewChild

### Community 106 - "HeaderComponent"
Cohesion: 0.12
Nodes (4): HeaderComponent, Component, Input, Output

### Community 107 - ".Handle"
Cohesion: 0.09
Nodes (17): CancellationToken, Task, CreateGameReservationCommand, CancellationToken, Task, DeclineGameReservationCommand, CancellationToken, Task (+9 more)

### Community 108 - "GetGameDropdownListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetGameDropdownListQuery, GetGameDropdownListQueryHandler, GetGameDropdownListQueryModel

### Community 109 - "SpaceBookingComponent"
Cohesion: 0.09
Nodes (7): DiceRollerComponent, Component, Input, Output, SpaceBookingComponent, Component, ViewChild

### Community 110 - "ConsoleFileLogger"
Cohesion: 0.11
Nodes (10): DH.Database.MigrationUtility, StreamWriter, bool, ConsoleFileLogger, EnvironmentSettings, Assembly, IServiceCollection, List (+2 more)

### Community 111 - "ReservationStatus"
Cohesion: 0.15
Nodes (15): IJobExecutionContext, Task, CloseActiveTablesJob, CancellationToken, List, Task, GetSpaceTableReservationHistoryQuery, GetSpaceTableReservationHistoryQueryHandler (+7 more)

### Community 112 - ".UploadQrCode"
Cohesion: 0.14
Nodes (12): DH.Domain.Models.ScannerModels.Queries, ActionAuthorize, CancellationToken, HttpPost, IActionResult, ProducesResponseType, Task, ScannerController (+4 more)

### Community 113 - ".EnsureRoleAsync"
Cohesion: 0.15
Nodes (12): IConfiguration, IServiceCollection, AuthenticationDIModule, IServiceProvider, RoleManager, Task, UserManager, ApplicationDbContextSeeder (+4 more)

### Community 114 - "DataRepository"
Cohesion: 0.31
Nodes (8): CancellationToken, Expression, Func, IEnumerable, List, Task, TenantDbContext, DataRepository

### Community 115 - "DH.Domain.Models.StatisticsModels.Queries"
Cohesion: 0.07
Nodes (32): DH.Application.Statistics.Queries, DH.Domain.Models.StatisticsModels.Queries, CancellationToken, List, Task, GetChallengeHistoryLogQuery, GetChallengeHistoryLogQueryHandler, CancellationToken (+24 more)

### Community 116 - "DataSeeder"
Cohesion: 0.13
Nodes (10): IDbContextFactory, IEnumerable, ILogger, Task, DataSeeder, DataSeederSystemUserContext, Task, IDataSeeder (+2 more)

### Community 117 - "NavigationMenuComponent"
Cohesion: 0.15
Nodes (4): IMenuItemInterface, NavigationMenuComponent, Component, HostListener

### Community 118 - "IAuthenticationService"
Cohesion: 0.19
Nodes (6): ChangePasswordRequest, LoginRequest, ResetPasswordRequest, CancellationToken, Task, IAuthenticationService

### Community 119 - "Chart2Component"
Cohesion: 0.20
Nodes (3): Chart2Component, Component, ViewChild

### Community 120 - "SpaceTableReservationHistory"
Cohesion: 0.35
Nodes (3): ITableReservationHistory, SpaceTableReservationHistory, Component

### Community 121 - "QrCodeScannerComponent"
Cohesion: 0.21
Nodes (3): QrCodeScannerComponent, Component, ViewChild

### Community 122 - "IUniversalChallengeProcessing"
Cohesion: 0.17
Nodes (11): IJobExecutionContext, Task, EventChecker, IJobExecutionContext, ILogger, Task, UserChallengeTop3StreakTrackerJob, CancellationToken (+3 more)

### Community 123 - "GetAllSystemRewardListQueryHandler"
Cohesion: 0.25
Nodes (11): CancellationToken, List, Task, GetAllSystemRewardListQuery, GetAllSystemRewardListQueryHandler, CancellationToken, List, Task (+3 more)

### Community 124 - "IEventService"
Cohesion: 0.15
Nodes (16): CancellationToken, Task, GetEventByIdQuery, GetEventByIdQueryHandler, DateTime, ICollection, Event, DateTime (+8 more)

### Community 125 - "CompleteTenantSetupCommandHandler"
Cohesion: 0.24
Nodes (10): CancellationToken, ILogger, IMediator, Task, CompleteTenantSetupCommand, CompleteTenantSetupCommandHandler, CancellationToken, Task (+2 more)

### Community 126 - "AddUpdateClubSpaceComponent"
Cohesion: 0.10
Nodes (4): IAddSpaceTableDto, IUpdateSpaceTableDto, AddUpdateClubSpaceComponent, Component

### Community 127 - "GetEventListQueryModel"
Cohesion: 0.17
Nodes (17): CancellationToken, List, Task, GetEventListForStaffQuery, GetEventListForStaffQueryHandler, CancellationToken, List, Task (+9 more)

### Community 128 - "DH.Authentication.UserContext.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.1), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, net8.0, Swashbuckle.AspNetCore (6.6.2) (+5 more)

### Community 129 - ".SubmitInquiry"
Cohesion: 0.15
Nodes (11): CancellationToken, HttpPost, IActionResult, IMediator, Task, PartnerInquiriesController, int, List (+3 more)

### Community 130 - "DH.Statistics.WorkerService.csproj"
Cohesion: 0.12
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.0), Microsoft.Extensions.Logging (8.0.0), Microsoft.NET.Sdk, net8.0, Microsoft.NET.Sdk (+5 more)

### Community 131 - "VisitorsChartComponent"
Cohesion: 0.23
Nodes (3): Component, ViewChild, VisitorsChartComponent

### Community 133 - ".RefreshAccessTokenAsync"
Cohesion: 0.22
Nodes (5): Claim, IEnumerable, List, Task, RoleHelper

### Community 134 - "DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Http.Abstractions (2.3.0), Microsoft.EntityFrameworkCore (8.0.3), Microsoft.EntityFrameworkCore.Design (8.0.3), Microsoft.EntityFrameworkCore.SqlServer (8.0.3), Microsoft.EntityFrameworkCore.Tools (8.0.3), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.2), Microsoft.NET.Sdk (+5 more)

### Community 135 - "LoginComponent"
Cohesion: 0.08
Nodes (6): LoginComponent, Component, SelectClubComponent, Component, CredentialManager, CredentialManagerPlugin

### Community 136 - "GetGameListQueryModel"
Cohesion: 0.18
Nodes (16): CancellationToken, List, Task, GetGameListByCategoryIdQuery, GetGameListByCategoryIdQueryHandler, CancellationToken, List, Task (+8 more)

### Community 137 - "DH.Database.MigrationUtility.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2), NodaTime (3.2.2), Microsoft.NET.Sdk, Mapster (7.4.1-pre01), Microsoft.Extensions.Caching.Memory (8.0.1), Microsoft.Extensions.Configuration.Json (8.0.1) (+5 more)

### Community 138 - "GetActivityChartData"
Cohesion: 0.27
Nodes (8): CancellationToken, Task, GetActivityChartDataQuery, GetActivityChartDataQueryHandler, DateTime, List, ActivityLog, GetActivityChartData

### Community 139 - "scripts"
Cohesion: 0.12
Nodes (15): name, private, scripts, build, cap:open, cap:sync, ng, postinstall (+7 more)

### Community 140 - "DH.Domain.Repositories"
Cohesion: 0.10
Nodes (7): DH.Application.Common.Queries, DH.Domain.Models.Common, DH.Domain.Repositories, DH.Domain.Models.RewardModels.Queries, DH.Application.Rewards.Queries, DH.Domain.Models.SpaceManagementModels.Queries, DH.Application.SpaceManagement.Queries

### Community 142 - "InstallPromptComponent"
Cohesion: 0.18
Nodes (4): IBeforeInstallPromptEvent, InstallPlatform, InstallPromptComponent, Component

### Community 143 - "Tenant Isolation Plan"
Cohesion: 0.13
Nodes (14): 0. Tenant contract, 10. Completion criteria, 1. Reproduce and baseline the leak, 2. Resolve tenant context consistently, 3. Fix database connection isolation, 4. Verify and enforce PostgreSQL RLS, 5. Complete the entity model inventory, 6. Audit queries and caches (+6 more)

### Community 144 - "CredentialManagerPlugin"
Cohesion: 0.28
Nodes (7): CapacitorPlugin, CredentialManagerPlugin, Override, JSObject, Plugin, PluginCall, PluginMethod

### Community 145 - "IFileManagerClient"
Cohesion: 0.18
Nodes (10): IDbContextFactory, Task, GameSeeder, CancellationToken, IDbContextFactory, MemoryStream, Task, RewardService (+2 more)

### Community 146 - "AdminEventManagementComponent"
Cohesion: 0.11
Nodes (5): IEventListResult, AdminEventManagementComponent, Component, EventsLibraryComponent, Component

### Community 147 - "GetTenantListQueryModel"
Cohesion: 0.23
Nodes (11): CancellationToken, Task, GetTenantByIdQuery, GetTenantByIdQueryHandler, CancellationToken, List, Task, GetTenantListQuery (+3 more)

### Community 148 - "EventAttendanceByEventsChartComponent"
Cohesion: 0.21
Nodes (3): EventAttendanceByEventsChartComponent, Component, ViewChild

### Community 149 - "QRReaderModel"
Cohesion: 0.18
Nodes (13): CancellationToken, Task, CancellationToken, Task, CancellationToken, Task, CancellationToken, Exception (+5 more)

### Community 150 - "SchedulerService"
Cohesion: 0.15
Nodes (14): DailyTenantJobSpec, CancellationToken, DateTime, ILogger, List, Task, DailyTenantJobSpec, SchedulerService (+6 more)

### Community 151 - "DH.Database.Connector.csproj"
Cohesion: 0.14
Nodes (11): net8.0, Microsoft.EntityFrameworkCore (8.0.11), Microsoft.EntityFrameworkCore.SqlServer (8.0.11), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4), Microsoft.NET.Sdk, net8.0, Microsoft.EntityFrameworkCore.Design (8.0.11), Microsoft.EntityFrameworkCore.Tools (8.0.11) (+3 more)

### Community 152 - "DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs"
Cohesion: 0.23
Nodes (11): CancellationToken, DateTime, DayOfWeek, IDbContextFactory, List, Task, ChallengeHistoryLogType, DateTimeExtensions (+3 more)

### Community 153 - "options"
Cohesion: 0.19
Nodes (14): options, baseHref, browser, index, inlineStyleLanguage, outputPath, polyfills, scripts (+6 more)

### Community 154 - "CreateOwnerPasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateOwnerPasswordRequest, CreateOwnerPasswordComponent, Component

### Community 155 - "ISystemUserContextAccessor"
Cohesion: 0.17
Nodes (9): SystemUserContextAccessor, CancellationToken, IConfiguration, ILogger, Task, EmailHistorySystemUserContext, SendEmployeeCreatePasswordEmailCommand, SendEmployeeCreatePasswordEmailCommandHandler (+1 more)

### Community 156 - "EventAttendanceChartComponent"
Cohesion: 0.26
Nodes (3): EventAttendanceChartComponent, Component, ViewChild

### Community 157 - "reservation-management.module.ts"
Cohesion: 0.08
Nodes (14): ReservationHistoryActionsComponent, Component, ContentChild, Input, Output, ReservationHistoryFiltersComponent, Component, Output (+6 more)

### Community 158 - "DH.Adapter.Data.Migrations"
Cohesion: 0.15
Nodes (7): DH.Adapter.Data.Migrations, ModelBuilder, InitialSeedQuartzNET, ModelBuilder, InitialTenant, ModelBuilder, AddTenantApplications

### Community 159 - "Google Cloud Setup and Deployment Notes"
Cohesion: 0.15
Nodes (12): **1. Create a Google Cloud Project**, **2. Set Up a Virtual Machine (VM) on Google Cloud**, **3. Install .NET Core SDK and Runtime on the VM**, **4. Deploy the Migration Utility to the VM**, **5. Connect to the VM**, a. **Generate a New SSH Key Pair**, **Autofac Version Issue**, b. **Add the New Public Key to Your Google Cloud VM** (+4 more)

### Community 160 - "TenantDirectoryService"
Cohesion: 0.33
Nodes (6): CancellationToken, IDbContextFactory, List, Task, TenantDirectoryService, TenantScheduleInfo

### Community 161 - "QRCodeContext"
Cohesion: 0.22
Nodes (7): CancellationToken, Exception, IServiceScopeFactory, Task, QRCodeContext, CancellationToken, Task

### Community 162 - "EmailType.cs"
Cohesion: 0.36
Nodes (9): string, EmployeePasswordCreation, ForgotPasswordResetKeys, OwnerPasswordCreation, PartnerInquiryRequest, RegistrationEmailTemplateKeys, TenantApplicationEmailVerification, TenantOwnerCredentials (+1 more)

### Community 163 - "TenantApplicationDto"
Cohesion: 0.21
Nodes (12): CancellationToken, Task, GetTenantApplicationByIdQuery, GetTenantApplicationByIdQueryHandler, CancellationToken, List, Task, GetTenantApplicationsQuery (+4 more)

### Community 164 - ".GetUserLocalOrUtcTime"
Cohesion: 0.18
Nodes (7): DateTime, NotificationRendererExtensions, DateTime, TimeSpan, TimeZoneHelper, IsUtcFallback, LocalTime

### Community 165 - "ReservationType"
Cohesion: 0.24
Nodes (7): ReservationCleanupHelper, CancellationToken, DateTime, List, Task, ReservationCleanupQueue, ReservationType

### Community 166 - "AssistiveTouchComponent"
Cohesion: 0.22
Nodes (5): AssistiveTouchComponent, Component, HostListener, AssistiveTouchModule, NgModule

### Community 167 - "EventService"
Cohesion: 0.29
Nodes (7): CancellationToken, IDbContextFactory, List, MemoryStream, Task, EventService, UpdateEventResponseModel

### Community 169 - "MessagingService"
Cohesion: 0.15
Nodes (4): MessagingService, Injectable, JobsComponent, Component

### Community 170 - "GameReservationHistory"
Cohesion: 0.27
Nodes (3): IGameReservationHistory, GameReservationHistory, Component

### Community 171 - ".get"
Cohesion: 0.03
Nodes (23): GameReviewsService, Injectable, IGameReviewListResult, RewardsService, Injectable, IRewardDropdownResult, IRewardListResult, IUserChallengePeriodReward (+15 more)

### Community 172 - "DH.Statistics.Domain.Entities"
Cohesion: 0.06
Nodes (31): DH.Statistics.Domain.Enums, DH.Statistics.Domain.Entities, CancellationToken, DateTime, IDbContextFactory, List, Task, GetActivityChartDataQuery (+23 more)

### Community 173 - "CreateEmployeePasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateEmployeePasswordRequest, CreateEmployeePasswordComponent, Component

### Community 174 - "TenantDbConnectionInterceptor"
Cohesion: 0.33
Nodes (6): CancellationToken, ConnectionEndEventData, DbConnection, IHttpContextAccessor, Task, TenantDbConnectionInterceptor

### Community 175 - "GetGameActivityChartData"
Cohesion: 0.31
Nodes (7): CancellationToken, Task, GetGameActivityChartDataQuery, GetGameActivityChartDataQueryHandler, List, GameActivityStats, GetGameActivityChartData

### Community 176 - "ISchedulerService"
Cohesion: 0.14
Nodes (17): IJobExecutionContext, ILogger, Task, AddUserChallengePeriodJob, ActionAuthorize, CancellationToken, HttpGet, HttpPost (+9 more)

### Community 177 - "VenueApplicationComponent"
Cohesion: 0.13
Nodes (8): Component, VenueApplicationComponent, canvasToBlob(), disposeBitmap(), downscaleImageFile(), IDownscaleOptions, loadBitmap(), renameForType()

### Community 178 - ".GetGameCategoryList"
Cohesion: 0.09
Nodes (22): CancellationToken, IDbContextFactory, List, Task, GameCategoryService, ActionAuthorize, CancellationToken, HttpPost (+14 more)

### Community 179 - "MapPermissions"
Cohesion: 0.18
Nodes (7): UserAction, IUserContext, IActionPermissions, Dictionary, IDictionary, List, MapPermissions

### Community 181 - "GetSystemRewardDropdownListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSystemRewardDropdownListQuery, GetSystemRewardDropdownListQueryHandler, GetSystemRewardDropdownListQueryModel

### Community 182 - ".ValidateQRCodeAsync"
Cohesion: 0.25
Nodes (5): byte, CancellationToken, Task, QrCodeDecryptor, QRCodeManager

### Community 183 - "SendTenantApplicationEmailVerificationCodeCommandHandler"
Cohesion: 0.24
Nodes (7): CancellationToken, ILogger, IMemoryCache, int, Task, SendTenantApplicationEmailVerificationCodeCommand, SendTenantApplicationEmailVerificationCodeCommandHandler

### Community 184 - "GameService"
Cohesion: 0.31
Nodes (6): CancellationToken, List, MemoryStream, Task, TenantDbContext, GameService

### Community 185 - "IReservationExpirationHandler"
Cohesion: 0.27
Nodes (6): IJobExecutionContext, Task, ExpireReservationJob, CancellationToken, Task, IReservationExpirationHandler

### Community 186 - "DH.DiceHub.sln"
Cohesion: 0.18
Nodes (3): DH.Adapter.QRManager, net8.0, Microsoft.NET.Sdk

### Community 188 - "InitialTenant"
Cohesion: 0.22
Nodes (5): DH.Adapter.Authentication.Migrations, MigrationBuilder, ModelBuilder, InitialTenant, InitialTenant

### Community 189 - "DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj"
Cohesion: 0.18
Nodes (10): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.3), Microsoft.EntityFrameworkCore (8.0.4), Microsoft.EntityFrameworkCore.Design (8.0.4), Microsoft.EntityFrameworkCore.SqlServer (8.0.4), Microsoft.EntityFrameworkCore.Tools (8.0.4), Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4) (+2 more)

### Community 190 - "GetExpiredCollectedRewardsChartDataModel"
Cohesion: 0.31
Nodes (7): CancellationToken, Task, GetExpiredCollectedRewardsChartDataQuery, GetExpiredCollectedRewardsChartDataQueryHandler, List, GetExpiredCollectedRewardsChartDataModel, RewardsStats

### Community 191 - "SendTenantSetupInvitationCommandHandler"
Cohesion: 0.24
Nodes (7): CancellationToken, IConfiguration, ILogger, int, Task, SendTenantSetupInvitationCommand, SendTenantSetupInvitationCommandHandler

### Community 192 - "http"
Cohesion: 0.18
Nodes (10): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, profiles (+2 more)

### Community 193 - "GetCustomPeriodQueryModel"
Cohesion: 0.29
Nodes (9): CancellationToken, Task, GetCustomPeriodQuery, GetCustomPeriodQueryHandler, List, GetCustomPeriodChallengeQueryModel, GetCustomPeriodQueryModel, GetCustomPeriodRewardQueryModel (+1 more)

### Community 194 - "GameSessionQueue"
Cohesion: 0.27
Nodes (6): GameSessionHelper, CancellationToken, DateTime, List, Task, GameSessionQueue

### Community 195 - "GetRoomMessageListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomMessageListQuery, GetRoomMessageListQueryHandler, DateTime, GetRoomMessageListQueryModel

### Community 196 - "manifest.json"
Cohesion: 0.18
Nodes (10): background_color, description, display, icons, name, orientation, scope, short_name (+2 more)

### Community 199 - "ReservationsChartComponent"
Cohesion: 0.29
Nodes (3): ReservationsChartComponent, Component, ViewChild

### Community 200 - "IUserManagementService"
Cohesion: 0.14
Nodes (9): GetUserByRoleModel, UserRegistrationResponse, CancellationToken, Dictionary, List, Task, IUserManagementService, DateTime (+1 more)

### Community 201 - "UserChallengeValidationJob"
Cohesion: 0.33
Nodes (4): IJobExecutionContext, ILogger, Task, UserChallengeValidationJob

### Community 203 - "AssistiveTouchComponent"
Cohesion: 0.15
Nodes (6): AssistiveTouchSettings, AssistiveTouchComponent, Component, HostListener, Input, Output

### Community 204 - "SupabaseStorageClient"
Cohesion: 0.18
Nodes (8): Client, DH.Adapter.FileManager, IConfiguration, IServiceCollection, DI, IConfiguration, Task, SupabaseStorageClient

### Community 205 - "AddUserChallengePeriodHandler"
Cohesion: 0.60
Nodes (3): CancellationToken, Task, AddUserChallengePeriodHandler

### Community 206 - "UserRewardsExpirationReminderJob"
Cohesion: 0.24
Nodes (7): IJobExecutionContext, ILogger, Task, UserRewardsExpirationReminderJob, CancellationToken, Task, IUserRewardsExpirationReminderHandler

### Community 207 - "AuthTokenService"
Cohesion: 0.10
Nodes (8): HttpRequestInterceptor, Injectable, AuthRedirectGuard, Injectable, ColdBootRestoreGuard, Injectable, AuthTokenService, Injectable

### Community 208 - "GetUserActiveTableQueryHandler"
Cohesion: 0.31
Nodes (7): CancellationToken, ILogger, Task, GetUserActiveTableQuery, GetUserActiveTableQueryHandler, DateTime, GetUserActiveTableQueryModel

### Community 209 - "UserRewardsExpiryJob"
Cohesion: 0.24
Nodes (7): IJobExecutionContext, ILogger, Task, UserRewardsExpiryJob, CancellationToken, Task, IUserRewardsExpiryHandler

### Community 210 - "VerifyTenantApplicationEmailVerificationCodeCommandHandler"
Cohesion: 0.21
Nodes (8): Entry, TenantApplicationEmailVerificationCache, CancellationToken, IMemoryCache, int, Task, VerifyTenantApplicationEmailVerificationCodeCommand, VerifyTenantApplicationEmailVerificationCodeCommandHandler

### Community 211 - ".Handle"
Cohesion: 0.50
Nodes (4): CancellationToken, Task, DeleteGameReservationCommand, DeleteGameReservationCommandHandler

### Community 212 - "ChallengeHubClientProxy"
Cohesion: 0.36
Nodes (3): IHubContext, Task, ChallengeHubClientProxy

### Community 213 - "ToastService"
Cohesion: 0.05
Nodes (64): ICustomPeriodChallenge, ICustomPeriodReward, ICustomPeriodUniversalChallenge, TenantSettingsService, Injectable, IGameDropdownResult, ICustomPeriodForm, PeriodDataAction (+56 more)

### Community 214 - "DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj"
Cohesion: 0.22
Nodes (8): net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.NET.Sdk, Quartz.AspNetCore (3.13.0), Quartz.Extensions.DependencyInjection (3.13.0), Quartz.Extensions.Hosting (3.13.0), Quartz.Plugins (3.13.0), Quartz.Serialization.Json (3.13.0)

### Community 215 - "Run BE + FE in separate terminal windows"
Cohesion: 0.29
Nodes (6): 1. Preconditions (check first, one command), 2. Free the ports (only if 4200 / 5000 / 5001 are taken), 3. Launch the two windows, 4. Report, Notes, Run BE + FE in separate terminal windows

### Community 216 - "ITenantSettings"
Cohesion: 0.46
Nodes (4): IUserChallengePeriodPerformance, TimePeriodType, ITenantSettings, WeekDay

### Community 217 - "GetUserRewardListQueryHandler"
Cohesion: 0.39
Nodes (7): CancellationToken, List, Task, GetUserRewardListQuery, GetUserRewardListQueryHandler, GetUserRewardListQueryModel, UserRewardStatus

### Community 218 - "CreateGameReviewDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, CreateGameReviewDto

### Community 219 - "ActionAuthorizeFilter"
Cohesion: 0.25
Nodes (6): AuthorizationFilterContext, int, Task, ActionAuthorizeFilter, IUserActionService, IAsyncAuthorizationFilter

### Community 220 - ".TryDequeue"
Cohesion: 0.20
Nodes (8): CancellationToken, List, Task, IStatisticJobQueue, CancellationToken, List, Task, StatisticJobQueue

### Community 221 - "qr-code-scanner.component.ts"
Cohesion: 0.11
Nodes (17): ScannerService, Injectable, QrCodeType, IQrCode, IQrCodeRequest, IQrCodeValidationResult, Component, Inject (+9 more)

### Community 222 - "DH.Messaging.Publisher.csproj"
Cohesion: 0.22
Nodes (7): net8.0, Microsoft.Extensions.DependencyInjection (8.0.1), Microsoft.NET.Sdk, net8.0, Microsoft.Extensions.Hosting (8.0.1), Microsoft.NET.Sdk, RabbitMQ.Client (7.0.0)

### Community 223 - "production"
Cohesion: 0.22
Nodes (9): build, builder, configurations, defaultConfiguration, production, budgets, buildTarget, fileReplacements (+1 more)

### Community 224 - "development"
Cohesion: 0.22
Nodes (9): serve, development, buildTarget, extractLicenses, optimization, sourceMap, builder, configurations (+1 more)

### Community 225 - "GetRoomInfoMessageListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomInfoMessageListQuery, GetRoomInfoMessageListQueryHandler, DateTime, GetRoomInfoMessageListQueryModel

### Community 226 - "UpdateGameReviewDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, UpdateGameReviewDto

### Community 227 - "IRequest"
Cohesion: 0.04
Nodes (44): CancellationToken, Task, CreateChallengeCommand, CreateChallengeCommandHandler, CancellationToken, Task, GetAssistiveTouchSettingsQuery, GetAssistiveTouchSettingsQueryHandler (+36 more)

### Community 228 - "ScrollTopComponent"
Cohesion: 0.25
Nodes (5): ScrollTopComponent, Component, HostListener, ScrollToTopModule, NgModule

### Community 229 - "UpdateRoomCommandDto"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, UpdateRoomCommandDto

### Community 230 - "PermissionStringBuilder"
Cohesion: 0.32
Nodes (5): IMemoryCache, PermissionStringBuilder, IDictionary, List, IMapPermissions

### Community 231 - ".JobWasExecuted"
Cohesion: 0.25
Nodes (7): CancellationToken, IJobExecutionContext, IServiceScopeFactory, Task, JobListenerForDeadLetterQueue, JobExecutionException, JobListenerSupport

### Community 232 - ".Update"
Cohesion: 0.09
Nodes (21): CancellationToken, Task, ReservationExpirationHandler, CancellationToken, Task, UpdateChallengeCommand, UpdateChallengeCommandHandler, CancellationToken (+13 more)

### Community 233 - "GetUserChallengePeriodRewardListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetUserChallengePeriodRewardListQuery, GetUserChallengePeriodRewardListQueryHandler, GetUserChallengePeriodRewardListQueryModel

### Community 234 - "QrCodeValidationResult"
Cohesion: 0.22
Nodes (8): CancellationToken, Task, CancellationToken, Task, bool, QrCodeType, string, QrCodeValidationResult

### Community 235 - "GetSeedGameCatalogDropdownListQueryHandler"
Cohesion: 0.46
Nodes (6): CancellationToken, List, Task, GetSeedGameCatalogDropdownListQuery, GetSeedGameCatalogDropdownListQueryHandler, GetSeedGameCatalogDropdownListQueryModel

### Community 236 - ".GetByAsync"
Cohesion: 0.03
Nodes (78): CancellationToken, IEnumerable, ILogger, List, Task, PushNotificationsService, CancellationToken, Task (+70 more)

### Community 238 - "SpaceTableService"
Cohesion: 0.39
Nodes (5): CancellationToken, IDbContextFactory, List, Task, SpaceTableService

### Community 239 - "GetGameReviewListQueryHandler"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetGameReviewListQuery, GetGameReviewListQueryHandler, DateTime, GetGameReviewListQueryModel

### Community 240 - "GetGameReservedListQueryHandler"
Cohesion: 0.31
Nodes (8): CancellationToken, List, Task, GameRecord, GetGameReservedListQuery, GetGameReservedListQueryHandler, DateTime, GetGameReservationListQueryModel

### Community 241 - "Deploy DH.Api (deploy.sh)"
Cohesion: 0.40
Nodes (4): Deploy DH.Api (deploy.sh), Notes, Steps, Where it lives / where to run it

### Community 242 - "ISynchronizeUsersChallengesQueue"
Cohesion: 0.32
Nodes (5): CancellationToken, DateTime, List, Task, ISynchronizeUsersChallengesQueue

### Community 243 - "IQueuedJobService"
Cohesion: 0.20
Nodes (7): Task, StatisticQueuePublisher, CancellationToken, List, Task, IQueuedJobService, IDomainService

### Community 245 - "DH.WebUI"
Cohesion: 0.25
Nodes (8): prefix, projectType, root, schematics, sourceRoot, DH.WebUI, style, @schematics/angular:component

### Community 246 - "DHWebUI"
Cohesion: 0.25
Nodes (7): Build, Code scaffolding, Development server, DHWebUI, Further help, Running end-to-end tests, Running unit tests

### Community 247 - "LoadingIndicatorComponent"
Cohesion: 0.40
Nodes (4): LoadingIndicatorComponent, Component, ContentChild, Input

### Community 248 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, EventQRCodeState

### Community 250 - "ToastComponent"
Cohesion: 0.36
Nodes (3): ToastComponent, Component, Inject

### Community 251 - "ServerErrorComponent"
Cohesion: 0.22
Nodes (7): ServerErrorComponent, Component, ServerErrorModule, NgModule, routes, ServerErrorRoutingModule, NgModule

### Community 252 - "DH.Statistics.WorkerService.Common"
Cohesion: 0.38
Nodes (4): DH.Statistics.WorkerService.Common, RabbitMqOptions, RabbitMqQueues, RabbitMqRoutingKeys

### Community 253 - "PasswordVisibilityToggleComponent"
Cohesion: 0.33
Nodes (4): PasswordVisibilityToggleComponent, Component, Input, Output

### Community 254 - "DH.Adapter.Email"
Cohesion: 0.29
Nodes (7): DH.Adapter.Email, net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, Microsoft.Extensions.Options (8.0.2), Microsoft.Extensions.Options.ConfigurationExtensions (8.0.0)

### Community 255 - "SynchronizeUsersChallengesQueue"
Cohesion: 0.32
Nodes (5): CancellationToken, DateTime, List, Task, SynchronizeUsersChallengesQueue

### Community 258 - "GetGameInventoryQueryHandler"
Cohesion: 0.43
Nodes (5): CancellationToken, Task, GetGameInventoryQuery, GetGameInventoryQueryHandler, GetGameInvetoryQueryModel

### Community 259 - "IGameSessionService"
Cohesion: 0.48
Nodes (3): CancellationToken, Task, IGameSessionService

### Community 260 - "DH.DiceHub.IntegrationTests"
Cohesion: 0.29
Nodes (7): DH.DiceHub.IntegrationTests, net8.0, Microsoft.NET.Sdk, Microsoft.NET.Test.Sdk (17.10.0), Npgsql (8.0.3), xunit (2.8.1), xunit.runner.visualstudio (2.8.1)

### Community 261 - "angular.json"
Cohesion: 0.29
Nodes (6): cli, analytics, newProjectRoot, projects, $schema, version

### Community 262 - "architect"
Cohesion: 0.29
Nodes (7): extract-i18n, test, architect, builder, options, buildTarget, builder

### Community 263 - "assets"
Cohesion: 0.29
Nodes (7): assets, src/favicon.ico, src/firebase-messaging-sw.js, src/manifest.json, src/shared/assets, src/shared/assets/images, src/.well-known

### Community 264 - "IChatHubClient"
Cohesion: 0.22
Nodes (5): DH.Domain.Adapters.ChatHub, IServiceCollection, ChatHubDIModule, Task, IChatHubClient

### Community 265 - "RandomColorDirective"
Cohesion: 0.33
Nodes (3): RandomColorDirective, Input, Directive

### Community 266 - "OperationResult"
Cohesion: 0.04
Nodes (45): DH.OperationResultCore.Extension, DH.Statistics.Application, DH.Statistics.Application.Commands, Dictionary, IError, List, OperationResultExtension, bool (+37 more)

### Community 267 - ".Handle"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomMemberListQuery, GetRoomMemberListQueryHandler, DateTime, GetRoomMemberListQueryModel

### Community 268 - "DH.Adapter.FileManager"
Cohesion: 0.33
Nodes (6): DH.Adapter.FileManager, net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.NET.Sdk, Supabase (1.1.1), Supabase.Storage (2.4.1)

### Community 270 - "challenges-management.module.ts"
Cohesion: 0.05
Nodes (23): AdminChallengesHistoryLogComponent, Component, StreakLeaderboardComponent, Component, StreakRewardsComponent, Component, StreakComponent, StreakPageType (+15 more)

### Community 271 - "GetUserWhoPlayedGameChartDataQueryHandler"
Cohesion: 0.29
Nodes (8): CancellationToken, Task, GetUserWhoPlayedGameChartDataQuery, GetUserWhoPlayedGameChartDataQueryHandler, DateTime, List, GameUserActivity, GetUsersWhoPlayedGameData

### Community 272 - "AppIdentityDbContextModelSnapshot.cs"
Cohesion: 0.17
Nodes (7): ModelBuilder, AppIdentityDbContextModelSnapshot, ModelBuilder, TenantDbContextModelSnapshot, ModelBuilder, StatisticsDbContextModelSnapshot, ModelSnapshot

### Community 274 - "GetActiveGameReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveGameReservationListQuery, GetActiveGameReservationListQueryHandler, DateTime, GetActiveGameReservationListQueryModel

### Community 275 - "GetAllEventsDropdownListQueryHandler"
Cohesion: 0.50
Nodes (6): CancellationToken, List, Task, GetAllEventsDropdownListModel, GetAllEventsDropdownListQuery, GetAllEventsDropdownListQueryHandler

### Community 277 - "GetActiveSpaceTableReservationCountQueryHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, GetActiveSpaceTableReservationCountQuery, GetActiveSpaceTableReservationCountQueryHandler

### Community 279 - "games-library.component.ts"
Cohesion: 0.04
Nodes (32): GameCategoriesService, Injectable, IGameCategory, IGameListResult, AddUpdateGameComponent, Component, ViewChild, GameDetailsComponent (+24 more)

### Community 280 - "ApiEndpoints.cs"
Cohesion: 0.40
Nodes (4): DH.Statistics.WorkerService, string, ApiEndpoints, Statistics

### Community 281 - "DH.Adapter.ChallengeHub"
Cohesion: 0.40
Nodes (5): DH.Adapter.ChallengeHub, net8.0, Microsoft.AspNetCore.SignalR (1.0.4), Microsoft.Extensions.DependencyInjection (8.0.0), Microsoft.NET.Sdk

### Community 282 - ".AddDataAdapter"
Cohesion: 0.60
Nodes (3): IConfiguration, IServiceCollection, DataDIModule

### Community 283 - "Migration"
Cohesion: 0.24
Nodes (5): MigrationBuilder, InitialSeedQuartzNET, MigrationBuilder, InitialCreate, Migration

### Community 292 - "DH.Adapter.Localization"
Cohesion: 0.40
Nodes (5): DH.Adapter.Localization, net8.0, Microsoft.NET.Sdk, Microsoft.AspNetCore.Localization (2.3.0), Microsoft.Extensions.Localization (8.0.19)

### Community 293 - "DH.Adapter.PushNotifications"
Cohesion: 0.40
Nodes (5): DH.Adapter.PushNotifications, net8.0, Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, FirebaseAdmin (3.0.1)

### Community 294 - "DH.Adapter.Statistics"
Cohesion: 0.40
Nodes (5): DH.Adapter.Statistics, net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.Extensions.Hosting.Abstractions (8.0.1), Microsoft.NET.Sdk

### Community 296 - "club-space-management.module.ts"
Cohesion: 0.08
Nodes (23): ClubSpaceManagementModule, NgModule, ClubSpaceManagementRoutingModule, NgModule, EventsLibraryModule, NgModule, EventsLibraryRoutingModule, NgModule (+15 more)

### Community 298 - "GetGameByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetGameByIdQuery, GetGameByIdQueryHandler, GetGameByIdQueryModel

### Community 300 - "GetGameReservationHistoryQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetGameReservationHistoryQuery, GetGameReservationHistoryQueryHandler, DateTime, GetGameReservationHistoryQueryModel

### Community 302 - ".GetActiveUserCustomPeriod"
Cohesion: 0.60
Nodes (3): ILogger, List, UserChallengePeriodPerformanceExtensions

### Community 303 - "GetSpaceActivityStatsQueryHandler"
Cohesion: 0.36
Nodes (6): CancellationToken, ILogger, Task, GetSpaceActivityStatsQuery, GetSpaceActivityStatsQueryHandler, GetSpaceActivityStatsQueryModel

### Community 305 - "GetSpaceAvailableTableListQuery"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSpaceAvailableTableListQuery, GetSpaceAvailableTableListQueryHandler, GetSpaceAvailableTableListQueryModel

### Community 307 - "RegisterComponent"
Cohesion: 0.12
Nodes (4): RegisterComponent, Component, GlobalErrorHandler, Injectable

### Community 310 - "Models/Common/RabbitMqOptions.cs"
Cohesion: 0.83
Nodes (3): RabbitMqOptions, RabbitMqQueues, RabbitMqRoutingKeys

### Community 311 - "GetChallengeListWithFilterQuery"
Cohesion: 0.52
Nodes (6): CancellationToken, List, Task, GetChallengeListWithFilterQuery, GetChallengeListWithFilterQueryHandler, GetChallengeListWithFilterQueryModel

### Community 312 - "GetUniversalChallengeListQueryHandler"
Cohesion: 0.52
Nodes (6): CancellationToken, List, Task, GetUniversalChallengeListQuery, GetUniversalChallengeListQueryHandler, GetUniversalChallengeListQueryModel

### Community 313 - "GetUserChallengePeriodPerformanceQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetUserChallengePeriodPerformanceQuery, GetUserChallengePeriodPerformanceQueryHandler, DateTime, GetUserChallengePeriodPerformanceQueryModel

### Community 315 - "gradlew"
Cohesion: 0.83
Nodes (3): gradlew script, die(), warn()

### Community 316 - "GetActiveReservedGameQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetActiveReservedGameQuery, GetActiveReservedGameQueryHandler, DateTime, GetActiveReservedGameQueryModel

### Community 318 - "MainActivity.java"
Cohesion: 0.47
Nodes (4): BridgeActivity, Bundle, Override, MainActivity

### Community 321 - "GetGameReservationStatusQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetGameReservationStatusQuery, GetGameReservationStatusQueryHandler, DateTime, GetGameReservationStatusQueryModel

### Community 322 - "GetGameReservationByIdQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetGameReservationByIdQuery, GetGameReservationByIdQueryHandler, DateTime, GetGameReservationByIdQueryModel

### Community 327 - "GetActiveBookedSpaceTableQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetActiveBookedSpaceTableQuery, GetActiveBookedSpaceTableQueryHandler, DateTime, GetActiveBookedSpaceTableQueryModel

### Community 332 - "GetSpaceTableByIdQueryHandler"
Cohesion: 0.43
Nodes (5): CancellationToken, Task, GetSpaceTableByIdQuery, GetSpaceTableByIdQueryHandler, GetSpaceTableByIdQueryModel

### Community 334 - "GetSpaceTableReservationByIdQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetSpaceTableReservationByIdQuery, GetSpaceTableReservationByIdQueryHandler, DateTime, GetSpaceTableReservationByIdQueryModel

### Community 341 - "GetSpaceTableParticipantListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetSpaceTableParticipantListQuery, GetSpaceTableParticipantListQueryHandler, GetSpaceTableParticipantListQueryModel

### Community 344 - ".getClubName"
Cohesion: 0.12
Nodes (5): IClubNameResult, ConfirmEmailComponent, Component, LanguageSwitchComponent, Component

### Community 347 - "ScanConfirmDialogComponent"
Cohesion: 0.29
Nodes (3): ScanConfirmDialogComponent, Component, Inject

### Community 366 - ".AddSchedulingAdapter"
Cohesion: 0.47
Nodes (4): IConfiguration, IServiceCollection, SchedulingDIModule, IServiceCollectionQuartzConfigurator

### Community 367 - "GetChallengeByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetChallengeByIdQuery, GetChallengeByIdQueryHandler, GetChallengeByIdQueryModel

### Community 368 - "ValidateTenantSetupTokenQueryHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, ValidateTenantSetupTokenQuery, ValidateTenantSetupTokenQueryHandler

### Community 369 - "DeleteGameCommandHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, DeleteGameCommand, DeleteGameCommandHandler

### Community 370 - "GetActiveGameReservationCountQueryHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, GetActiveGameReservationCountQuery, GetActiveGameReservationCountQueryHandler

### Community 373 - "DH.Api/Program.cs"
Cohesion: 0.05
Nodes (23): DH.Application, DH.Adapter.Scheduling, DH.Api, DH.Adapter.Data, DH.Adapter.PushNotifications, DH.Domain, DH.Adapater.Localization, DH.Api.Filters (+15 more)

### Community 377 - "GameAveragePlaytime"
Cohesion: 0.17
Nodes (9): GameAveragePlaytime, int, List, ValidationError, CreateGameDto, int, List, ValidationError (+1 more)

### Community 378 - "NavBarComponent"
Cohesion: 0.40
Nodes (3): NavBarComponent, Component, Input

### Community 386 - "CreateRoomCommandDto"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, CreateRoomCommandDto

### Community 387 - "UserSettingsDto"
Cohesion: 0.27
Nodes (8): CancellationToken, Task, GetUserSettingsQuery, GetUserSettingsQueryHandler, bool, List, ValidationError, UserSettingsDto

### Community 391 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, UnknownQRCodeState

### Community 395 - "TokenService"
Cohesion: 0.24
Nodes (6): ClaimsPrincipal, DateTime, UserManager, TokenService, TimeSpan, JwtTokenOptions

## Knowledge Gaps
- **420 isolated node(s):** `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)`, `xunit.runner.visualstudio (2.8.1)`, `Npgsql (8.0.3)` (+415 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **68 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `DH.Domain.Entities` connect `DH.Domain.Entities` to `TenantDbContext`, `DH.Domain.Adapters.Data`, `DH.Domain.Adapters.Statistics.Services`, `DH.Domain.Services`, `DH.Domain.Adapters.Localization`, `DH.Domain.Repositories`, `DH.OperationResultCore.Exceptions`, `GetAllEventsDropdownListQueryHandler`, `DH.Domain.Adapters.Scheduling`, `DH.Domain.Models.GameModels.Queries`, `ChallengeReward`, `DH.Domain.Adapters.Authentication`, `GameSessionService`, `IEmailHelperService`, `UserChallengesManagementService`, `IUserManagementService`, `Tenant`, `QRCodeManager.cs`, `.GetGlobalTenantSettingsAsync`, `DH.Api/Program.cs`, `IEventService`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Why does `ILocalizationService` connect `IRequestHandler` to `.SubmitInquiry`, `CreateRoomCommandDto`, `UserSettingsDto`, `LocalizationService`, `ITenantDbContext`, `IValidableFields`, `TenantApplicationsController`, `IPushNotificationsService`, `GetActiveGameReservationListQueryHandler`, `ChallengesController`, `ChallengeReward`, `ISystemUserContextAccessor`, `.GetUserLocalOrUtcTime`, `UserManagementService`, `EmployeeService`, `IEmailHelperService`, `OwnerService`, `GameService`, `NotificationPayload`, `AuthenticationService`, `GetActiveSpaceTableReservationListQueryHandler`, `TenantSettingsController`, `IChallengeService`, `GetSpaceTableParticipantListQueryHandler`, `CreateGameReviewDto`, `SendRegistrationEmailConfirmationCommandHandler`, `GetRoomInfoMessageListQueryHandler`, `UpdateGameReviewDto`, `IRequest`, `UpdateRoomCommandDto`, `SpaceTableService`, `GameAveragePlaytime`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Why does `DH.Domain.Enums` connect `DH.Domain.Entities` to `TenantDbContext`, `DH.Domain.Adapters.Data`, `DH.Domain.Adapters.Statistics.Services`, `DH.Domain.Services`, `GetActivityChartData`, `DH.Domain.Adapters.Localization`, `DH.Domain.Repositories`, `TenantApplicationsController`, `DH.OperationResultCore.Exceptions`, `DH.Domain.Adapters.Scheduling`, `DH.Domain.Models.GameModels.Queries`, `ChallengeReward`, `DH.Domain.Adapters.Authentication`, `StatisticsService`, `TenantApplicationDto`, `ReservationType`, `GetGameActivityChartData`, `UserChallengesManagementService`, `Tenant`, `IChallengeService`, `QRCodeManager.cs`, `.GetGlobalTenantSettingsAsync`, `QueuedJob`, `ReservationStatus`, `GameAveragePlaytime`?**
  _High betweenness centrality (0.026) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)` to the rest of the system?**
  _420 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TenantRouter` be split into smaller, more focused modules?**
  _Cohesion score 0.039001937984496124 - nodes in this community are weakly interconnected._
- **Should `GamesService` be split into smaller, more focused modules?**
  _Cohesion score 0.0394655704008222 - nodes in this community are weakly interconnected._
- **Should `app.module.ts` be split into smaller, more focused modules?**
  _Cohesion score 0.07256894049346879 - nodes in this community are weakly interconnected._