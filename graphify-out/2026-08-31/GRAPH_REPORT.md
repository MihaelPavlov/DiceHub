# Graph Report - DiceHub  (2026-08-31)

## Corpus Check
- 1200 files · ~6,060,056 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 7405 nodes · 19133 edges · 402 communities (320 shown, 82 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 576 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `b6a3eb79`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- TenantRouter
- SpaceManagementService
- app.module.ts
- LandingComponent
- TenantDbContext
- ITenantDbContext
- DH.Domain.Adapters.Data
- DH.Domain.Entities
- GlobalSettingsComponent
- DH.Adapter.ChallengesOrchestrator
- DH.Domain.Adapters.FileManager
- IValidableFields
- DH.Domain.Adapters.Localization
- TenantApplicationsController
- StatisticsController
- .SendNotificationToUsersAsync
- AuthService
- .error
- DH.OperationResultCore.Exceptions
- ChallengesController
- TenantSettingsExtensions.cs
- GamesController
- DH.Domain.Repositories
- ChallengeReward
- DH.Domain.Adapters.Authentication
- .navigateTenant
- RoomsController
- DH.Domain.Models.Common
- StatisticsService
- MeepleRoomDetailsComponent
- DH.Statistics.Domain.Models.Queries
- LinkInfoComponent
- .get
- DH.Statistics.Data
- AdminChallengesCustomPeriodComponent
- IRequestHandler
- AppIdentityDbContext
- NotificationsController
- TenantApplicationsService
- challenges.service.ts
- UserManagementService
- UserController
- ReservationCleanupWorker
- EventAttendanceDetectedMessage
- AuthorizedHttpClient
- ChallengesManagementComponent
- AppComponent
- .GetGlobalTenantSettingsAsync
- statistics.service.ts
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
- ILocalizationService
- AuthenticationService
- GetActiveSpaceTableReservationListQueryHandler
- AdminChallengesListComponent
- DH.OperationResultCore.Utility
- ApiExceptionFilterAttribute
- http
- .onUpdateReview
- GetSystemRewardByIdQueryHandler
- meeple-room-details.component.ts
- ChallengeType
- dependencies
- devDependencies
- DH.DiceHub/DH.Domain/DH.Domain.csproj
- SchedulerService
- AdminChallengesSystemRewardsComponent
- RoomChatComponent
- GameLayoutComponent
- DH.Messaging.HttpClient.UserContext
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
- .CreateGameReview
- SendTenantOwnerCredentialsEmailCommandHandler
- ReservationManagementNavigationComponent
- EventsController
- TenantSetupService
- TenantDbConnectionInterceptor
- UniversalChallengeProcessing
- TenantsController
- .getCurrentLanguage
- .UpdateAssistiveTouchSettings
- RewardsController
- GetClubInfoQueryHandler
- AddUpdateGameComponent
- ISeedService
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
- .ChangePassword
- Chart2Component
- SpaceTableReservationHistory
- UserChallenge
- IUniversalChallengeProcessing
- .Handle
- IEventService
- CompleteTenantSetupCommandHandler
- AddUpdateClubSpaceComponent
- GetEventListQueryModel
- DH.Authentication.UserContext.csproj
- ControllerBase
- DH.Statistics.WorkerService.csproj
- VisitorsChartComponent
- FrontendLogController
- TokenService
- DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj
- LoginComponent
- GetGameListQueryModel
- DH.Database.MigrationUtility.csproj
- .GetActivityChartData
- scripts
- DH.Domain.Models.SpaceManagementModels.Queries
- .post
- InstallPromptComponent
- Tenant Isolation Plan
- MeepleRoomMenuComponent
- RewardService
- EventsLibraryComponent
- GetTenantListQueryModel
- EventAttendanceByEventsChartComponent
- QRReaderModel
- ISchedulerService
- DH.Database.Connector.csproj
- DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs
- options
- .buildTenantUrl
- AdminEventManagementComponent
- EventAttendanceChartComponent
- ReservationHistoryActionsComponent
- DH.Adapter.Data.Migrations
- Google Cloud Setup and Deployment Notes
- ClubSpaceManagementComponent
- IQRCodeState
- EmailType.cs
- TenantApplicationDto
- .GetUserLocalOrUtcTime
- ReservationType
- AssistiveTouchComponent
- EventService
- .resetData
- MessagingService
- GameReservationHistory
- admin-challenges-system-rewards.component.ts
- DH.Statistics.Domain.Entities
- CreateEmployeePasswordComponent
- ValidationFilterAttribute
- ApiExceptionFilterAttribute
- SchedulerController
- VenueApplicationComponent
- .GetGameCategoryList
- MapPermissions
- IStatisticJobInfo
- IRabbitMqClient
- .ValidateQRCodeAsync
- SendTenantApplicationEmailVerificationCodeCommandHandler
- GameService
- IReservationExpirationHandler
- DH.DiceHub.sln
- DH.Adapter.Authentication.Migrations
- DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj
- GetExpiredCollectedRewardsChartDataModel
- SendTenantSetupInvitationCommandHandler
- http
- GetCustomPeriodQueryModel
- GameSessionQueue
- GetRoomMessageListQueryHandler
- manifest.json
- GamesChartComponent
- UserRegistrationRequest
- ReservationsChartComponent
- IUserManagementService
- IValidableFields
- RoomMembersComponent
- AssistiveTouchComponent
- IFileManagerClient
- RewardsCollectedChartComponent
- UserRewardsExpirationReminderJob
- AuthTokenService
- GetUserActiveTableQueryHandler
- UserRewardsExpiryJob
- VerifyTenantApplicationEmailVerificationCodeCommandHandler
- Task
- ChallengeHubClientProxy
- ToastService
- DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj
- Run BE + FE in separate terminal windows
- ITenantSettings
- GetUserRewardListQueryHandler
- GetExpiredCollectedRewardsChartDataQuery
- ActionAuthorizeFilter
- .TryDequeue
- QrCodeScannerComponent
- DH.Messaging.Publisher.csproj
- production
- development
- GetRoomInfoMessageListQueryHandler
- ClubSpaceListComponent
- GetAssistiveTouchSettingsQueryHandler
- ScrollTopComponent
- GetEventByIdQueryModel
- PermissionStringBuilder
- .JobWasExecuted
- IRequest
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
- DH.Messaging.HttpClient.Enums
- DH.WebUI
- DHWebUI
- ReservationProcessingOutcomeMessage
- NotificationTypeRegistry
- IUserContext
- ToastComponent
- LanguageService
- DH.Statistics.WorkerService.Common
- PasswordVisibilityToggleComponent
- DH.Adapter.Email
- SynchronizeUsersChallengesQueue
- EmailHelperService
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
- UpdateTenantSettingsCommandHandler
- StreakComponent
- GetUserWhoPlayedGameChartDataQueryHandler
- TenantDbContextModelSnapshot.cs
- 20260729093650_AddSeedGameCatalog.Designer.cs
- GetActiveGameReservationListQueryHandler
- GetAllEventsDropdownListQueryHandler
- GetEventListForUserQueryHandler
- GetActiveSpaceTableReservationCountQueryHandler
- EventMessage
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
- StreakRewardsComponent
- ChipComponent
- 20260118090517_InitialData.Designer.cs
- GetGameByIdQueryHandler
- ErrorInterceptor
- GetGameReservationHistoryQueryHandler
- DiceRollerComponent
- .GetActiveUserCustomPeriod
- GetSpaceActivityStatsQueryHandler
- InitialSeedQuartzNET
- GetSpaceAvailableTableListQuery
- GetUserStatsQueryHandler
- ForgotPasswordComponent
- RegisterChoiceComponent
- Models/Common/RabbitMqOptions.cs
- GetChallengeListWithFilterQuery
- GetUniversalChallengeListQueryHandler
- GetUserChallengePeriodPerformanceQueryHandler
- ExampleInstrumentedTest
- gradlew
- GetActiveReservedGameQueryHandler
- CalculateRemainingDaysPipe
- BridgeActivity
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
- ResetPasswordComponent
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
- ConfirmEmailComponent
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
- PartnerInquiryDto
- StreakLeaderboardComponent
- DH.Adapter.Reservations
- CreateGameCommandHandler
- UpdateGameCommandHandler
- @angular/common
- CreateGameDto
- NavBarComponent
- @angular/platform-browser
- @capacitor/app
- chartjs-adapter-date-fns
- date-fns
- firebase
- @ngx-translate/http-loader
- UpdateRewardDto
- CreateRoomCommandDto
- UserSettingsDto
- JobsComponent
- LocalizationService
- .HandleAsync
- .HandleAsync
- ChallengeProcessingOutcomeMessage
- DI
- Models/Enums/Role.cs
- JwtTokenOptions
- SynchronizeUsersChallengesQueueHelper.cs
- StatisticJobQueueHelper.cs
- .FieldsAreValid
- QueueNameKeysConstants.cs
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

## Communities (402 total, 82 thin omitted)

### Community 0 - "TenantRouter"
Cohesion: 0.04
Nodes (24): EventsService, Injectable, GameCategoriesService, Injectable, GamesService, Injectable, Injectable, UsersService (+16 more)

### Community 1 - "SpaceManagementService"
Cohesion: 0.05
Nodes (47): ActiveReservedGame, ICreateGameReservation, IGameReservationHistory, IGameReservationStatus, IGetReservationById, IReservedGame, SpaceManagementService, Injectable (+39 more)

### Community 2 - "app.module.ts"
Cohesion: 0.03
Nodes (88): AppRoutingModule, NgModule, ROUTES, AdminChallengesHistoryLogComponent, Component, CustomPeriodLeaveConfirmationDialog, Component, SinglePlayerConfirmDialog (+80 more)

### Community 4 - "TenantDbContext"
Cohesion: 0.03
Nodes (57): CancellationToken, DbContextOptionsBuilder, DbSet, IHttpContextAccessor, ModelBuilder, Task, TenantDbContext, DateTime (+49 more)

### Community 5 - "ITenantDbContext"
Cohesion: 0.08
Nodes (24): CancellationToken, Task, CreateGameReviewCommand, CreateGameReviewCommandHandler, CancellationToken, Task, DeleteGameReviewByIdCommand, DeleteGameReviewByIdCommandHandler (+16 more)

### Community 6 - "DH.Domain.Adapters.Data"
Cohesion: 0.04
Nodes (26): DH.Adapter.Data.Repositories, DH.Application, DH.Application.Games.Seeders, DH.Application.Games.Commands.Games, DH.Api, DH.Adapter.Data, DH.Domain, DH.Adapter.Data.Seeder (+18 more)

### Community 7 - "DH.Domain.Entities"
Cohesion: 0.04
Nodes (27): DH.Domain.Adapters.Statistics.Services, DH.Adapter.Data.Services, DH.Adapter.GameSession, DH.Domain.Adapters.Statistics, DH.Domain.Queue, DH.Application.SpaceManagement.Commands, DH.Adapter.Statistics, DH.Domain.Entities (+19 more)

### Community 8 - "GlobalSettingsComponent"
Cohesion: 0.07
Nodes (9): ToggleState, GlobalSettingsComponent, Component, canvasToBlob(), disposeBitmap(), downscaleImageFile(), IDownscaleOptions, loadBitmap() (+1 more)

### Community 9 - "DH.Adapter.ChallengesOrchestrator"
Cohesion: 0.40
Nodes (3): DH.Adapter.ChallengesOrchestrator, IServiceCollection, ChallengesOrchestratorAdapterDI

### Community 10 - "DH.Domain.Adapters.FileManager"
Cohesion: 0.10
Nodes (9): DH.Domain.Models.EventModels.Queries, DH.Adapter.FileManager, DH.Domain.Adapters.FileManager, DH.Application.Events.Queries, DH.Domain.Models.EventModels.Command, IConfiguration, IServiceCollection, DI (+1 more)

### Community 11 - "IValidableFields"
Cohesion: 0.05
Nodes (31): DateTime, List, ValidationError, CreateEventModel, DateTime, List, ValidationError, UpdateEventModel (+23 more)

### Community 12 - "DH.Domain.Adapters.Localization"
Cohesion: 0.07
Nodes (18): DH.Adapter.ChallengeHub, DH.Domain.Adapters.PushNotifications.Messages.Models, DH.Domain.Adapters.PushNotifications, DH.Domain.Adapters.Localization, DH.Domain.Adapters.ChallengeHub, DH.Adapter.PushNotifications, DH.Domain.Models, DH.Domain.Adapters.PushNotifications.Messages.Common (+10 more)

### Community 13 - "TenantApplicationsController"
Cohesion: 0.17
Nodes (21): ActionAuthorize, AllowAnonymous, Authorize, CancellationToken, HttpGet, HttpPost, IActionResult, IFormFile (+13 more)

### Community 14 - "StatisticsController"
Cohesion: 0.46
Nodes (7): CancellationToken, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticsController

### Community 15 - ".SendNotificationToUsersAsync"
Cohesion: 0.08
Nodes (25): ConcurrentDictionary, Exception, IHubContext, Task, ChallengeHubClient, CancellationToken, Task, CreateEventCommand (+17 more)

### Community 16 - "AuthService"
Cohesion: 0.04
Nodes (61): TODO: Check this tread…, AuthService, Injectable, UserRole, IRegisterRequest, IRegisterResponse, ITokenResponse, IUserInfo (+53 more)

### Community 18 - "DH.OperationResultCore.Exceptions"
Cohesion: 0.05
Nodes (25): DH.Domain.Models.RoomModels.Commands, DH.Domain.Adapters.Scheduling, DH.Adapter.Scheduling, DH.OperationResultCore.Exceptions, DH.Domain.Models.RewardModels.Commands, DH.Adapater.Localization, DH.Adapter.Scheduling.Jobs, DH.Api.Filters (+17 more)

### Community 19 - "ChallengesController"
Cohesion: 0.14
Nodes (21): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+13 more)

### Community 21 - "GamesController"
Cohesion: 0.26
Nodes (12): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+4 more)

### Community 22 - "DH.Domain.Repositories"
Cohesion: 0.05
Nodes (15): DH.Domain.Models.ChallengeModels.Queries, DH.Application.Challenges.Qureies, DH.Domain.Models.GameModels.Queries, DH.Domain.Repositories, DH.Application.Games.Queries, DH.Domain.Models.RewardModels.Queries, DH.Domain.Models.ChallengeModels.Commands, DH.Domain.Services (+7 more)

### Community 23 - "ChallengeReward"
Cohesion: 0.12
Nodes (17): CancellationToken, Task, CreateSystemRewardCommand, CreateSystemRewardCommandHandler, CancellationToken, Task, UpdateSystemRewardCommand, UpdateSystemRewardCommandHandler (+9 more)

### Community 24 - "DH.Domain.Adapters.Authentication"
Cohesion: 0.07
Nodes (18): DH.Domain.Adapters.QRManager.StateModels, DH.Adapter.Authentication.Helper, DH.Domain.Adapters.Authentication.Options, DH.Adapter.QRManager, DH.Adapter.Authentication, DH.Domain.Adapters.Authentication.Interfaces, DH.Adapter.Authentication.Entities, DH.Domain.Adapters.Authentication (+10 more)

### Community 25 - ".navigateTenant"
Cohesion: 0.05
Nodes (10): EventsChartsLayoutComponent, Component, RewardChartsLayoutComponent, Component, EmployeeListComponent, Component, InstructionManagementComponent, Component (+2 more)

### Community 26 - "RoomsController"
Cohesion: 0.07
Nodes (41): CancellationToken, IDbContextFactory, List, Task, RoomService, ActionAuthorize, CancellationToken, HttpDelete (+33 more)

### Community 27 - "DH.Domain.Models.Common"
Cohesion: 0.08
Nodes (15): DH.Application.Common.Queries, DH.Domain.Models.Common, DH.Application.Common.Commands, DH.Adapter.Authentication.Filters, DH.Adapter.Email, DH.Api.Controllers, DH.Domain.Adapters.Email, DH.Domain.Adapters.Authentication.Enums (+7 more)

### Community 28 - "StatisticsService"
Cohesion: 0.10
Nodes (16): CancellationToken, DateTime, IDbContextFactory, List, Task, StatisticsService, Test, ChartActivityType (+8 more)

### Community 29 - "MeepleRoomDetailsComponent"
Cohesion: 0.16
Nodes (3): MeepleRoomDetailsComponent, Component, ViewChild

### Community 30 - "DH.Statistics.Domain.Models.Queries"
Cohesion: 0.08
Nodes (30): DH.Statistics.Application.Queries, DH.Statistics.Api.Controllers, DH.Statistics.Domain.Models.Queries, CancellationToken, IDbContextFactory, List, Task, GetCollectedRewardsByDatesQuery (+22 more)

### Community 31 - "LinkInfoComponent"
Cohesion: 0.07
Nodes (18): INSTRUCTION_LINK_MAPPINGS, InstructionSection, InstructionStep, InstructionTopic, LinkInfoType, StepActionLink, InstructionComponent, Component (+10 more)

### Community 32 - ".get"
Cohesion: 0.04
Nodes (15): ITenantListResult, SchedulerService, Injectable, SuperadminTenantDetailsComponent, Component, SuperadminTenantsComponent, Component, SelectClubComponent (+7 more)

### Community 33 - "DH.Statistics.Data"
Cohesion: 0.06
Nodes (26): DH.Statistics.Data.Migrations, DH.Statistics.Data, DH.Statistics.Application.Commands, CancellationToken, IDbContextFactory, Task, CreateClubVisitorLogCommand, CreateClubVisitorLogCommandHandler (+18 more)

### Community 34 - "AdminChallengesCustomPeriodComponent"
Cohesion: 0.06
Nodes (7): ICustomPeriod, IUniversalChallengeDropdownResult, AdminChallengesCustomPeriodComponent, customPeriodValidator(), Component, CanComponentDeactivate, canDeactivateGuard()

### Community 35 - "IRequestHandler"
Cohesion: 0.07
Nodes (61): SystemUserContextAccessor, UserContext, GameReservationQRCodeState, TableReservationQRCodeState, UserRewardsExpirationReminderHandler, ILogger, AddUserChallengePeriodJob, IMemoryCache (+53 more)

### Community 36 - "AppIdentityDbContext"
Cohesion: 0.08
Nodes (18): CancellationToken, DbContextOptionsBuilder, ModelBuilder, Task, AppIdentityDbContext, IConfiguration, AppIdentityDbContextFactory, IConfiguration (+10 more)

### Community 37 - "NotificationsController"
Cohesion: 0.45
Nodes (8): ActionAuthorize, CancellationToken, HttpGet, HttpPost, IActionResult, ProducesResponseType, Task, NotificationsController

### Community 38 - "TenantApplicationsService"
Cohesion: 0.07
Nodes (21): TenantApplicationsService, Injectable, ICompleteTenantSetupRequest, ICompleteTenantSetupResult, ISeedGameCatalogDropdown, ITenantApplication, ITenantApplicationRequest, ITenantApplicationReviewRequest (+13 more)

### Community 39 - "challenges.service.ts"
Cohesion: 0.17
Nodes (12): ChallengeRewardPoint, ChallengeStatus, IChallengeResult, IChallengeListResult, ICreateChallengeDto, IUniversalChallengeListResult, IUpdateChallengeDto, IUpdateUniversalChallengeDto (+4 more)

### Community 40 - "UserManagementService"
Cohesion: 0.15
Nodes (10): CancellationToken, Dictionary, ILogger, List, RoleManager, Task, UserManager, UserManagementService (+2 more)

### Community 41 - "UserController"
Cohesion: 0.19
Nodes (18): ActionAuthorize, AllowAnonymous, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut (+10 more)

### Community 42 - "ReservationCleanupWorker"
Cohesion: 0.07
Nodes (29): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, SynchronizeUsersChallengesWorker, CancellationToken, ILogger (+21 more)

### Community 43 - "EventAttendanceDetectedMessage"
Cohesion: 0.67
Nodes (3): DateTime, AttendanceAction, EventAttendanceDetectedMessage

### Community 44 - "AuthorizedHttpClient"
Cohesion: 0.08
Nodes (23): CancellationToken, HttpMethod, IHttpClientFactory, ILogger, JsonSerializerOptions, string, StringContent, Task (+15 more)

### Community 45 - "ChallengesManagementComponent"
Cohesion: 0.12
Nodes (6): IUserCustomPeriodChallenge, IUserCustomPeriodReward, ChallengesManagementComponent, Component, ViewChild, ViewChildren

### Community 46 - "AppComponent"
Cohesion: 0.16
Nodes (5): AppComponent, Component, ViewChild, app, messaging

### Community 47 - ".GetGlobalTenantSettingsAsync"
Cohesion: 0.13
Nodes (22): completedChallenge, completedUniversalChallenges, CancellationToken, IDbContextFactory, IDbContextTransaction, IEnumerable, ILogger, List (+14 more)

### Community 48 - "statistics.service.ts"
Cohesion: 0.06
Nodes (21): ChallengeLeaderboardType, ChartActivityType, GamesActivityType, ActivityLog, GetActivityChartData, IChallengeLeaderboard, GetCollectedRewardsByDates, EventAttendance (+13 more)

### Community 49 - "IGameService"
Cohesion: 0.23
Nodes (10): DateTime, ICollection, Game, DateTime, GameReservation, CancellationToken, List, MemoryStream (+2 more)

### Community 50 - "SpaceManagementController"
Cohesion: 0.28
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 51 - "EmployeeService"
Cohesion: 0.11
Nodes (15): CancellationToken, ILogger, RoleManager, Task, UserManager, EmployeeService, CreateEmployeePasswordRequest, EmployeeModel (+7 more)

### Community 52 - "IStatisticsService"
Cohesion: 0.04
Nodes (45): DH.Domain.Adapters.Statistics.JobHandlers, CancellationToken, List, Task, GetCollectedRewardsByDatesQuery, GetCollectedRewardsByDatesQueryHandler, CancellationToken, Task (+37 more)

### Community 53 - "IRabbitMqUserContext"
Cohesion: 0.08
Nodes (17): BasicDeliverEventArgs, BasicProperties, DH.Messaging.Publisher.Extensions, DH.Messaging.Publisher.Authentication, IRabbitMqUserContext, IRabbitMqUserContextFactory, RabbitMqUserContext, RabbitMqUserContextFactory (+9 more)

### Community 54 - "IEmailHelperService"
Cohesion: 0.09
Nodes (25): CancellationToken, IConfiguration, ILogger, Task, EmailHistorySystemUserContext, SendEmployeeCreatePasswordEmailCommand, SendEmployeeCreatePasswordEmailCommandHandler, CancellationToken (+17 more)

### Community 55 - "OwnerService"
Cohesion: 0.12
Nodes (14): CancellationToken, ILogger, RoleManager, Task, UserManager, OwnerService, PasswordGenerator, CreateOwnerForTenantSetupRequest (+6 more)

### Community 56 - "UserContextFactory"
Cohesion: 0.07
Nodes (19): IHttpContextAccessor, Task, UserContextFactory, IMemoryCache, Task, UserSettingsCache, CancellationToken, Task (+11 more)

### Community 57 - "UserChallengesManagementService"
Cohesion: 0.12
Nodes (18): DbUpdateException, CancellationToken, IDbContextFactory, IDbContextTransaction, ILogger, List, Task, TenantDbContext (+10 more)

### Community 58 - ".RunAsTenantAsync"
Cohesion: 0.08
Nodes (22): Func, Task, TenantContextScopeRunner, Task, ChatHubClient, CancellationToken, Task, ReservationExpirationHandler (+14 more)

### Community 59 - "ILocalizationService"
Cohesion: 0.04
Nodes (49): ILocalizationService, ChallengeCompletedNotification, ChallengeUpdatedNotification, RenderableNotification, DateTime, EventDeletedNotification, DateTime, EventReminderNotification (+41 more)

### Community 60 - "AuthenticationService"
Cohesion: 0.11
Nodes (16): DateTime, ApplicationUser, CancellationToken, Task, UserManager, AuthenticationService, TokenResponseModel, Claim (+8 more)

### Community 61 - "GetActiveSpaceTableReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveSpaceTableReservationListQuery, GetActiveSpaceTableReservationListQueryHandler, DateTime, GetActiveSpaceTableReservationListQueryModel

### Community 62 - "AdminChallengesListComponent"
Cohesion: 0.08
Nodes (8): PartnerInquiriesService, Injectable, IPartnerInquiryRequest, ICreateEventDto, IUpdateEventDto, IAddUpdateRoomDto, AdminChallengesListComponent, Component

### Community 63 - "DH.OperationResultCore.Utility"
Cohesion: 0.11
Nodes (19): DH.Messaging.Publisher.Messages, DH.Messaging.HttpClient, DH.ServiceBusWorker, DH.OperationResultCore.Utility, DH.Statistics.WorkerService.Handlers, DH.Messaging.Publisher, IAuthorizedClientFactory, IServiceBusHandler (+11 more)

### Community 64 - "ApiExceptionFilterAttribute"
Cohesion: 0.25
Nodes (5): ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute, ExceptionFilterAttribute

### Community 65 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 67 - "GetSystemRewardByIdQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, Task, GetSystemRewardByIdQuery, GetSystemRewardByIdQueryHandler, GetRewardByIdQueryModel

### Community 68 - "meeple-room-details.component.ts"
Cohesion: 0.08
Nodes (27): AppModule, NgModule, RoomsService, Injectable, IRoomByIdResult, IRoomListResult, IRoomMemberResult, IRoomMessageResult (+19 more)

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

### Community 73 - "SchedulerService"
Cohesion: 0.06
Nodes (42): DailyTenantJobSpec, CancellationToken, IDbContextFactory, List, Task, TenantDirectoryService, CancellationToken, DateTime (+34 more)

### Community 74 - "AdminChallengesSystemRewardsComponent"
Cohesion: 0.08
Nodes (8): RewardLevel, REWARD_POINTS, RewardRequiredPoint, ICreateRewardDto, IRewardGetByIdResult, IUpdateRewardDto, AdminChallengesSystemRewardsComponent, Component

### Community 75 - "RoomChatComponent"
Cohesion: 0.15
Nodes (6): IRoomInfoMessageResult, GroupedChatMessage, IGroupMessage, RoomChatComponent, Component, ViewChild

### Community 76 - "GameLayoutComponent"
Cohesion: 0.18
Nodes (5): GameLayoutComponent, Component, Input, Output, NavItemInterface

### Community 77 - "DH.Messaging.HttpClient.UserContext"
Cohesion: 0.13
Nodes (8): DH.Messaging.HttpClient.UserContext, DH.Authentication.UserContext, IServiceCollection, DI, Role, int, string, UserContext

### Community 78 - "TenantIsolationFixture"
Cohesion: 0.16
Nodes (13): DH.DiceHub.IntegrationTests, int, string, Task, TenantIsolationFixture, Task, TenantIsolationTests, Fact (+5 more)

### Community 79 - "Tenant"
Cohesion: 0.09
Nodes (18): IMemoryCache, Task, TenantDbContext, TimeSpan, TenantService, HttpContext, Task, TenantRouteValidationMiddleware (+10 more)

### Community 80 - "IGameSessionQueue"
Cohesion: 0.07
Nodes (27): CancellationToken, Task, DeleteGameReservationCommand, DeleteGameReservationCommandHandler, ILogger, CloseSpaceTableCommand, CloseSpaceTableCommandHandler, CancellationToken (+19 more)

### Community 81 - "IChallengeService"
Cohesion: 0.05
Nodes (49): CancellationToken, IDbContextFactory, List, Task, ChallengeService, CancellationToken, Task, CreateChallengeCommand (+41 more)

### Community 83 - "StatisticController"
Cohesion: 0.24
Nodes (14): CancellationToken, HttpDelete, HttpPost, IActionResult, IMediator, ProducesResponseType, Task, StatisticController (+6 more)

### Community 84 - "NotificationsDialog"
Cohesion: 0.10
Nodes (7): NotificationsService, Injectable, IUserNotification, NotificationsDialog, Component, Inject, ViewChild

### Community 85 - "AddUpdateEventComponent"
Cohesion: 0.11
Nodes (6): AddUpdateEventComponent, futureDateValidator(), isFutureDate(), parseDateInput(), Component, ViewChild

### Community 86 - "GameAvailabilityComponent"
Cohesion: 0.07
Nodes (5): AddUpdateMeepleRoomComponent, futureDateValidator(), Component, GameAvailabilityComponent, Component

### Community 87 - "DH.Statistics.Api/Filters/ApiExceptionFilterAttribute.cs"
Cohesion: 0.22
Nodes (5): DH.OperationResultCore, DH.Statistics.Api.Filters, DH.Statistics.Application, Dictionary, IError

### Community 88 - "EventDetailsComponent"
Cohesion: 0.12
Nodes (4): AdminEventDetailsComponent, Component, EventDetailsComponent, Component

### Community 89 - "IUserChallengesManagementService"
Cohesion: 0.20
Nodes (8): CancellationToken, Task, AddUserChallengePeriodHandler, IJobExecutionContext, Task, CancellationToken, Task, IUserChallengesManagementService

### Community 90 - ".CreateGameReview"
Cohesion: 0.28
Nodes (11): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IMediator (+3 more)

### Community 91 - "SendTenantOwnerCredentialsEmailCommandHandler"
Cohesion: 0.10
Nodes (18): ILogger, SmtpEmailSender, CancellationToken, IConfiguration, ILogger, Task, CreatePartnerInquiriesCommand, CreatePartnerInquiriesCommandHandle (+10 more)

### Community 92 - "ReservationManagementNavigationComponent"
Cohesion: 0.09
Nodes (5): SpaceTableActiveReservations, Component, ReservationManagementNavigationComponent, Component, ViewChild

### Community 93 - "EventsController"
Cohesion: 0.32
Nodes (12): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+4 more)

### Community 94 - "TenantSetupService"
Cohesion: 0.21
Nodes (10): CancellationToken, List, Task, TenantDbContext, TenantSetupService, TenantSetupSystemUserContext, DateTime, SeedGameCatalog (+2 more)

### Community 95 - "TenantDbConnectionInterceptor"
Cohesion: 0.14
Nodes (14): DbConnectionInterceptor, CancellationToken, ConnectionEndEventData, DbConnection, HttpContext, IHttpContextAccessor, Task, ApplicationDbConnectionInterceptor (+6 more)

### Community 96 - "UniversalChallengeProcessing"
Cohesion: 0.25
Nodes (9): CancellationToken, IDbContextFactory, ILogger, Task, TenantDbContext, UniversalChallengeProcessing, DateTime, TenantSetting (+1 more)

### Community 97 - "TenantsController"
Cohesion: 0.29
Nodes (10): ActionResult, AllowAnonymous, Authorize, CancellationToken, HttpGet, IActionResult, IMediator, ProducesResponseType (+2 more)

### Community 98 - ".getCurrentLanguage"
Cohesion: 0.09
Nodes (6): ChallengeHubService, Injectable, ChallengeOverlayComponent, Component, LanguageSwitchComponent, Component

### Community 99 - ".UpdateAssistiveTouchSettings"
Cohesion: 0.32
Nodes (10): ActionAuthorize, CancellationToken, HttpGet, HttpPost, HttpPut, IActionResult, IMediator, ProducesResponseType (+2 more)

### Community 100 - "RewardsController"
Cohesion: 0.15
Nodes (23): ActionAuthorize, CancellationToken, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult, IFormFile (+15 more)

### Community 101 - "GetClubInfoQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetClubInfoModel, GetClubInfoQuery, GetClubInfoQueryHandler

### Community 102 - "AddUpdateGameComponent"
Cohesion: 0.11
Nodes (6): GameAveragePlaytime, ICreateGameDto, IUpdateGameDto, AddUpdateGameComponent, Component, ViewChild

### Community 103 - "ISeedService"
Cohesion: 0.12
Nodes (11): IMediator, Task, ChallengesSeedService, IMediator, Task, GamesSeedService, IMediator, Task (+3 more)

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
Nodes (16): CancellationToken, Task, CreateGameReservationCommand, CancellationToken, Task, ApproveSpaceTableReservationCommand, CancellationToken, Task (+8 more)

### Community 108 - "GetGameDropdownListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetGameDropdownListQuery, GetGameDropdownListQueryHandler, GetGameDropdownListQueryModel

### Community 109 - "SpaceBookingComponent"
Cohesion: 0.11
Nodes (7): DiceRollerComponent, Component, Input, Output, SpaceBookingComponent, Component, ViewChild

### Community 110 - "ConsoleFileLogger"
Cohesion: 0.11
Nodes (10): DH.Database.MigrationUtility, StreamWriter, bool, ConsoleFileLogger, EnvironmentSettings, Assembly, IServiceCollection, List (+2 more)

### Community 111 - "ReservationStatus"
Cohesion: 0.13
Nodes (18): IJobExecutionContext, Task, CloseActiveTablesJob, CancellationToken, List, Task, GetSpaceTableReservationHistoryQuery, GetSpaceTableReservationHistoryQueryHandler (+10 more)

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
Nodes (30): DH.Application.Statistics.Queries, DH.Domain.Models.StatisticsModels.Queries, CancellationToken, List, Task, GetChallengeHistoryLogQuery, GetChallengeHistoryLogQueryHandler, CancellationToken (+22 more)

### Community 116 - "DataSeeder"
Cohesion: 0.18
Nodes (8): IDbContextFactory, IEnumerable, ILogger, Task, DataSeeder, DataSeederSystemUserContext, Task, IDataSeeder

### Community 117 - "NavigationMenuComponent"
Cohesion: 0.13
Nodes (4): IMenuItemInterface, NavigationMenuComponent, Component, HostListener

### Community 118 - ".ChangePassword"
Cohesion: 0.17
Nodes (6): ChangePasswordRequest, LoginRequest, ResetPasswordRequest, CancellationToken, Task, IAuthenticationService

### Community 119 - "Chart2Component"
Cohesion: 0.20
Nodes (3): Chart2Component, Component, ViewChild

### Community 121 - "UserChallenge"
Cohesion: 0.13
Nodes (13): DateTime, ICollection, Challenge, ChallengeStatistic, CustomPeriodUniversalChallenge, DateTime, CustomPeriodUserUniversalChallenge, DateTime (+5 more)

### Community 122 - "IUniversalChallengeProcessing"
Cohesion: 0.17
Nodes (11): IJobExecutionContext, Task, EventChecker, IJobExecutionContext, ILogger, Task, UserChallengeTop3StreakTrackerJob, CancellationToken (+3 more)

### Community 123 - ".Handle"
Cohesion: 0.50
Nodes (4): CancellationToken, Task, DeleteSystemRewardCommand, DeleteSystemRewardCommandHandler

### Community 124 - "IEventService"
Cohesion: 0.21
Nodes (10): DateTime, ICollection, Event, DateTime, EventNotification, CancellationToken, List, MemoryStream (+2 more)

### Community 125 - "CompleteTenantSetupCommandHandler"
Cohesion: 0.24
Nodes (10): CancellationToken, ILogger, IMediator, Task, CompleteTenantSetupCommand, CompleteTenantSetupCommandHandler, CancellationToken, Task (+2 more)

### Community 126 - "AddUpdateClubSpaceComponent"
Cohesion: 0.11
Nodes (4): IAddSpaceTableDto, IUpdateSpaceTableDto, AddUpdateClubSpaceComponent, Component

### Community 127 - "GetEventListQueryModel"
Cohesion: 0.23
Nodes (12): CancellationToken, List, Task, GetEventListForStaffQuery, GetEventListForStaffQueryHandler, CancellationToken, List, Task (+4 more)

### Community 128 - "DH.Authentication.UserContext.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.1), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, net8.0, Swashbuckle.AspNetCore (6.6.2) (+5 more)

### Community 129 - "ControllerBase"
Cohesion: 0.20
Nodes (9): ControllerBase, IMediator, GameCategoriesController, CancellationToken, HttpPost, IActionResult, IMediator, Task (+1 more)

### Community 130 - "DH.Statistics.WorkerService.csproj"
Cohesion: 0.12
Nodes (13): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.11), Microsoft.AspNetCore.Http.Abstractions (2.1.1), Microsoft.Extensions.Http (8.0.0), Microsoft.Extensions.Logging (8.0.0), Microsoft.NET.Sdk, net8.0, Microsoft.NET.Sdk (+5 more)

### Community 131 - "VisitorsChartComponent"
Cohesion: 0.23
Nodes (3): Component, ViewChild, VisitorsChartComponent

### Community 132 - "FrontendLogController"
Cohesion: 0.30
Nodes (7): DH.OperationResultCore.FrontEndErrors, CancellationToken, HttpPost, IActionResult, ILogger, FrontendLogController, ErrorBody

### Community 133 - "TokenService"
Cohesion: 0.22
Nodes (8): Claim, ClaimsPrincipal, DateTime, IEnumerable, List, Task, UserManager, TokenService

### Community 134 - "DH.DiceHub/DH.Adapter.Data/DH.Adapter.Data.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.AspNetCore.Http.Abstractions (2.3.0), Microsoft.EntityFrameworkCore (8.0.3), Microsoft.EntityFrameworkCore.Design (8.0.3), Microsoft.EntityFrameworkCore.SqlServer (8.0.3), Microsoft.EntityFrameworkCore.Tools (8.0.3), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.2), Microsoft.NET.Sdk (+5 more)

### Community 135 - "LoginComponent"
Cohesion: 0.07
Nodes (5): IClubNameResult, LoginComponent, Component, RegisterComponent, Component

### Community 136 - "GetGameListQueryModel"
Cohesion: 0.18
Nodes (16): CancellationToken, List, Task, GetGameListByCategoryIdQuery, GetGameListByCategoryIdQueryHandler, CancellationToken, List, Task (+8 more)

### Community 137 - "DH.Database.MigrationUtility.csproj"
Cohesion: 0.13
Nodes (13): net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2), NodaTime (3.2.2), Microsoft.NET.Sdk, Mapster (7.4.1-pre01), Microsoft.Extensions.Caching.Memory (8.0.1), Microsoft.Extensions.Configuration.Json (8.0.1) (+5 more)

### Community 138 - ".GetActivityChartData"
Cohesion: 0.26
Nodes (8): CancellationToken, Task, GetActivityChartDataQuery, GetActivityChartDataQueryHandler, DateTime, List, ActivityLog, GetActivityChartData

### Community 139 - "scripts"
Cohesion: 0.12
Nodes (15): name, private, scripts, build, cap:open, cap:sync, ng, postinstall (+7 more)

### Community 140 - "DH.Domain.Models.SpaceManagementModels.Queries"
Cohesion: 0.17
Nodes (3): DH.Application.Stats.Queries, DH.Domain.Models.SpaceManagementModels.Queries, DH.Application.SpaceManagement.Queries

### Community 141 - ".post"
Cohesion: 0.09
Nodes (4): ISpaceTableById, ISpaceTableParticipant, ClubSpaceDetailsComponent, Component

### Community 142 - "InstallPromptComponent"
Cohesion: 0.18
Nodes (4): IBeforeInstallPromptEvent, InstallPlatform, InstallPromptComponent, Component

### Community 143 - "Tenant Isolation Plan"
Cohesion: 0.13
Nodes (14): 0. Tenant contract, 10. Completion criteria, 1. Reproduce and baseline the leak, 2. Resolve tenant context consistently, 3. Fix database connection isolation, 4. Verify and enforce PostgreSQL RLS, 5. Complete the entity model inventory, 6. Audit queries and caches (+6 more)

### Community 144 - "MeepleRoomMenuComponent"
Cohesion: 0.23
Nodes (5): MeepleRoomMenuComponent, Component, HostListener, Input, Output

### Community 145 - "RewardService"
Cohesion: 0.43
Nodes (5): CancellationToken, IDbContextFactory, MemoryStream, Task, RewardService

### Community 147 - "GetTenantListQueryModel"
Cohesion: 0.23
Nodes (11): CancellationToken, Task, GetTenantByIdQuery, GetTenantByIdQueryHandler, CancellationToken, List, Task, GetTenantListQuery (+3 more)

### Community 148 - "EventAttendanceByEventsChartComponent"
Cohesion: 0.21
Nodes (3): EventAttendanceByEventsChartComponent, Component, ViewChild

### Community 149 - "QRReaderModel"
Cohesion: 0.17
Nodes (14): CancellationToken, Task, CancellationToken, Task, PurchaseChallengeQRCodeState, CancellationToken, Task, CancellationToken (+6 more)

### Community 150 - "ISchedulerService"
Cohesion: 0.24
Nodes (7): CancellationToken, DateTime, List, Task, ISchedulerService, DateTime, ScheduleJobInfo

### Community 151 - "DH.Database.Connector.csproj"
Cohesion: 0.14
Nodes (11): net8.0, Microsoft.EntityFrameworkCore (8.0.11), Microsoft.EntityFrameworkCore.SqlServer (8.0.11), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4), Microsoft.NET.Sdk, net8.0, Microsoft.EntityFrameworkCore.Design (8.0.11), Microsoft.EntityFrameworkCore.Tools (8.0.11) (+3 more)

### Community 152 - "DH.Statistics.Application/Queries/GetChallengeHistoryLogQuery.cs"
Cohesion: 0.23
Nodes (11): CancellationToken, DateTime, DayOfWeek, IDbContextFactory, List, Task, ChallengeHistoryLogType, DateTimeExtensions (+3 more)

### Community 153 - "options"
Cohesion: 0.19
Nodes (14): options, baseHref, browser, index, inlineStyleLanguage, outputPath, polyfills, scripts (+6 more)

### Community 154 - ".buildTenantUrl"
Cohesion: 0.06
Nodes (7): initializeUserFactory(), ICreateOwnerPasswordRequest, IResetPasswordRequest, CreateOwnerPasswordComponent, Component, ChallengeAdminAccessGuard, Injectable

### Community 156 - "EventAttendanceChartComponent"
Cohesion: 0.26
Nodes (3): EventAttendanceChartComponent, Component, ViewChild

### Community 157 - "ReservationHistoryActionsComponent"
Cohesion: 0.14
Nodes (5): ReservationHistoryActionsComponent, Component, ContentChild, Input, Output

### Community 158 - "DH.Adapter.Data.Migrations"
Cohesion: 0.15
Nodes (7): DH.Adapter.Data.Migrations, ModelBuilder, InitialSeedQuartzNET, ModelBuilder, InitialTenant, ModelBuilder, AddTenantApplications

### Community 159 - "Google Cloud Setup and Deployment Notes"
Cohesion: 0.15
Nodes (12): **1. Create a Google Cloud Project**, **2. Set Up a Virtual Machine (VM) on Google Cloud**, **3. Install .NET Core SDK and Runtime on the VM**, **4. Deploy the Migration Utility to the VM**, **5. Connect to the VM**, a. **Generate a New SSH Key Pair**, **Autofac Version Issue**, b. **Add the New Public Key to Your Google Cloud VM** (+4 more)

### Community 160 - "ClubSpaceManagementComponent"
Cohesion: 0.21
Nodes (3): getKeyFriendlyNames(), ClubSpaceManagementComponent, Component

### Community 161 - "IQRCodeState"
Cohesion: 0.16
Nodes (11): CancellationToken, Exception, IServiceScopeFactory, Task, QRCodeContext, CancellationToken, Task, EventQRCodeState (+3 more)

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
Cohesion: 0.24
Nodes (8): CancellationToken, IDbContextFactory, List, MemoryStream, Task, EventService, Task, UpdateEventResponseModel

### Community 171 - "admin-challenges-system-rewards.component.ts"
Cohesion: 0.04
Nodes (40): GameReviewsService, Injectable, IGameReviewListResult, QrCodeType, IQrCodeValidationResult, RewardsService, Injectable, IRewardDropdownResult (+32 more)

### Community 172 - "DH.Statistics.Domain.Entities"
Cohesion: 0.04
Nodes (42): DH.Database.Connector.Models, DH.Database.Connector, DH.Statistics.Domain.Enums, DH.Statistics.Domain.Entities, DbContext, Assembly, IConfiguration, IServiceCollection (+34 more)

### Community 173 - "CreateEmployeePasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateEmployeePasswordRequest, CreateEmployeePasswordComponent, Component

### Community 174 - "ValidationFilterAttribute"
Cohesion: 0.18
Nodes (7): ActionExecutedContext, ActionExecutingContext, ValidationFilterAttribute, ActionExecutedContext, ActionExecutingContext, ValidationFilterAttribute, IActionFilter

### Community 175 - "ApiExceptionFilterAttribute"
Cohesion: 0.27
Nodes (4): ExceptionContext, IDictionary, ILogger, ApiExceptionFilterAttribute

### Community 176 - "SchedulerController"
Cohesion: 0.26
Nodes (10): ILogger, UserChallengeValidationJob, ActionAuthorize, CancellationToken, HttpGet, HttpPost, IActionResult, ProducesResponseType (+2 more)

### Community 178 - ".GetGameCategoryList"
Cohesion: 0.09
Nodes (22): CancellationToken, IDbContextFactory, List, Task, GameCategoryService, ActionAuthorize, CancellationToken, HttpPost (+14 more)

### Community 179 - "MapPermissions"
Cohesion: 0.18
Nodes (7): UserAction, IUserContext, IActionPermissions, Dictionary, IDictionary, List, MapPermissions

### Community 180 - "IStatisticJobInfo"
Cohesion: 0.17
Nodes (13): StatisticJobFactory, CancellationToken, Task, IStatisticJob, IStatisticJobInfo, IStatisticJobFactory, StatisticJobType, ChallengeProcessingOutcomeJob (+5 more)

### Community 181 - "IRabbitMqClient"
Cohesion: 0.18
Nodes (9): Func, Task, IRabbitMqClient, CancellationToken, IServiceProvider, IServiceScope, string, Task (+1 more)

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

### Community 188 - "DH.Adapter.Authentication.Migrations"
Cohesion: 0.20
Nodes (5): DH.Adapter.Authentication.Migrations, ModelBuilder, InitialTenant, ModelBuilder, AppIdentityDbContextModelSnapshot

### Community 189 - "DH.DiceHub/DH.Adapter.Authentication/DH.Adapter.Authentication.csproj"
Cohesion: 0.18
Nodes (10): net8.0, Microsoft.AspNetCore.Authentication.JwtBearer (8.0.3), Microsoft.EntityFrameworkCore (8.0.4), Microsoft.EntityFrameworkCore.Design (8.0.4), Microsoft.EntityFrameworkCore.SqlServer (8.0.4), Microsoft.EntityFrameworkCore.Tools (8.0.4), Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Npgsql.EntityFrameworkCore.PostgreSQL (8.0.4) (+2 more)

### Community 190 - "GetExpiredCollectedRewardsChartDataModel"
Cohesion: 0.29
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

### Community 198 - "UserRegistrationRequest"
Cohesion: 0.25
Nodes (5): int, List, ValidationError, UserRegistrationRequest, UserRegistrationResponse

### Community 199 - "ReservationsChartComponent"
Cohesion: 0.29
Nodes (3): ReservationsChartComponent, Component, ViewChild

### Community 200 - "IUserManagementService"
Cohesion: 0.13
Nodes (15): CancellationToken, IConfiguration, ILogger, Task, RegistrationEmailSystemUserContext, SendRegistrationEmailConfirmationCommand, SendRegistrationEmailConfirmationCommandHandler, GetUserByRoleModel (+7 more)

### Community 201 - "IValidableFields"
Cohesion: 0.17
Nodes (9): List, ValidationError, CreateEmployeeRequest, List, ValidationError, CreateOwnerRequest, List, ValidationError (+1 more)

### Community 203 - "AssistiveTouchComponent"
Cohesion: 0.13
Nodes (8): TenantUserSettingsService, Injectable, AssistiveTouchSettings, AssistiveTouchComponent, Component, HostListener, Input, Output

### Community 204 - "IFileManagerClient"
Cohesion: 0.14
Nodes (10): Client, IDbContextFactory, Task, GameSeeder, IConfiguration, Task, SupabaseStorageClient, IFileManagerClient (+2 more)

### Community 205 - "RewardsCollectedChartComponent"
Cohesion: 0.31
Nodes (3): RewardsCollectedChartComponent, Component, ViewChild

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

### Community 211 - "Task"
Cohesion: 0.23
Nodes (5): CancellationToken, IEnumerable, Task, DateTime, GetUserNotificationsModel

### Community 212 - "ChallengeHubClientProxy"
Cohesion: 0.36
Nodes (3): IHubContext, Task, ChallengeHubClientProxy

### Community 213 - "ToastService"
Cohesion: 0.04
Nodes (72): ChallengesService, Injectable, ICustomPeriodChallenge, ICustomPeriodReward, ICustomPeriodUniversalChallenge, IUserSettings, IGameByIdResult, IGameDropdownResult (+64 more)

### Community 214 - "DH.DiceHub/DH.Adapter.Scheduling/DH.Adapter.Scheduling.csproj"
Cohesion: 0.22
Nodes (8): net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.NET.Sdk, Quartz.AspNetCore (3.13.0), Quartz.Extensions.DependencyInjection (3.13.0), Quartz.Extensions.Hosting (3.13.0), Quartz.Plugins (3.13.0), Quartz.Serialization.Json (3.13.0)

### Community 215 - "Run BE + FE in separate terminal windows"
Cohesion: 0.29
Nodes (6): 1. Preconditions (check first, one command), 2. Free the ports (only if 4200 / 5000 / 5001 are taken), 3. Launch the two windows, 4. Report, Notes, Run BE + FE in separate terminal windows

### Community 216 - "ITenantSettings"
Cohesion: 0.27
Nodes (4): IUserChallengePeriodPerformance, TimePeriodType, ITenantSettings, WeekDay

### Community 217 - "GetUserRewardListQueryHandler"
Cohesion: 0.39
Nodes (7): CancellationToken, List, Task, GetUserRewardListQuery, GetUserRewardListQueryHandler, GetUserRewardListQueryModel, UserRewardStatus

### Community 218 - "GetExpiredCollectedRewardsChartDataQuery"
Cohesion: 0.27
Nodes (8): CancellationToken, IDbContextFactory, Task, GetExpiredCollectedRewardsChartDataQuery, GetExpiredCollectedRewardsChartDataQueryHandler, List, GetExpiredCollectedRewardsChartDataModel, RewardsStats

### Community 219 - "ActionAuthorizeFilter"
Cohesion: 0.25
Nodes (6): AuthorizationFilterContext, int, Task, ActionAuthorizeFilter, IUserActionService, IAsyncAuthorizationFilter

### Community 220 - ".TryDequeue"
Cohesion: 0.17
Nodes (9): CancellationToken, List, Task, IStatisticJobQueue, CancellationToken, List, Task, StatisticJobQueue (+1 more)

### Community 221 - "QrCodeScannerComponent"
Cohesion: 0.07
Nodes (16): ScannerService, Injectable, IQrCode, IQrCodeRequest, Component, Inject, UserRewardQrCodeDialog, Inject (+8 more)

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

### Community 226 - "ClubSpaceListComponent"
Cohesion: 0.24
Nodes (3): ISpaceTableList, ClubSpaceListComponent, Component

### Community 227 - "GetAssistiveTouchSettingsQueryHandler"
Cohesion: 0.43
Nodes (5): CancellationToken, Task, GetAssistiveTouchSettingsQuery, GetAssistiveTouchSettingsQueryHandler, AssistiveTouchSettings

### Community 228 - "ScrollTopComponent"
Cohesion: 0.25
Nodes (5): ScrollTopComponent, Component, HostListener, ScrollToTopModule, NgModule

### Community 229 - "GetEventByIdQueryModel"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetEventByIdQuery, GetEventByIdQueryHandler, DateTime, GetEventByIdQueryModel

### Community 230 - "PermissionStringBuilder"
Cohesion: 0.32
Nodes (5): IMemoryCache, PermissionStringBuilder, IDictionary, List, IMapPermissions

### Community 231 - ".JobWasExecuted"
Cohesion: 0.25
Nodes (7): CancellationToken, IJobExecutionContext, IServiceScopeFactory, Task, JobListenerForDeadLetterQueue, JobExecutionException, JobListenerSupport

### Community 232 - "IRequest"
Cohesion: 0.05
Nodes (40): CancellationToken, Task, UpdateChallengeCommand, UpdateChallengeCommandHandler, CancellationToken, Task, UpdateUniversalChallengeCommand, UpdateUniversalChallengeCommandHandler (+32 more)

### Community 233 - "GetUserChallengePeriodRewardListQueryHandler"
Cohesion: 0.43
Nodes (6): CancellationToken, List, Task, GetUserChallengePeriodRewardListQuery, GetUserChallengePeriodRewardListQueryHandler, GetUserChallengePeriodRewardListQueryModel

### Community 234 - "QrCodeValidationResult"
Cohesion: 0.29
Nodes (7): CancellationToken, Task, RewardQRCodeState, bool, QrCodeType, string, QrCodeValidationResult

### Community 235 - "GetSeedGameCatalogDropdownListQueryHandler"
Cohesion: 0.46
Nodes (6): CancellationToken, List, Task, GetSeedGameCatalogDropdownListQuery, GetSeedGameCatalogDropdownListQueryHandler, GetSeedGameCatalogDropdownListQueryModel

### Community 236 - ".GetByAsync"
Cohesion: 0.04
Nodes (65): CancellationToken, IEnumerable, ILogger, List, Task, PushNotificationsService, CancellationToken, Task (+57 more)

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

### Community 244 - "DH.Messaging.HttpClient.Enums"
Cohesion: 0.10
Nodes (12): DH.Messaging.HttpClient.Helpers, DH.Messaging.HttpClient.Enums, IHttpClientFactory, ILogger, AuthorizedClientFactory, IServiceCollection, DI, ApplicationApi (+4 more)

### Community 245 - "DH.WebUI"
Cohesion: 0.25
Nodes (8): prefix, projectType, root, schematics, sourceRoot, DH.WebUI, style, @schematics/angular:component

### Community 246 - "DHWebUI"
Cohesion: 0.25
Nodes (7): Build, Code scaffolding, Development server, DHWebUI, Further help, Running end-to-end tests, Running unit tests

### Community 247 - "ReservationProcessingOutcomeMessage"
Cohesion: 0.60
Nodes (4): DateTime, ReservationOutcome, ReservationProcessingOutcomeMessage, ReservationType

### Community 248 - "NotificationTypeRegistry"
Cohesion: 0.50
Nodes (3): Dictionary, Type, NotificationTypeRegistry

### Community 249 - "IUserContext"
Cohesion: 0.29
Nodes (5): IUserContext, IUserContextFactory, IHttpContextAccessor, ILogger, UserContextFactory

### Community 250 - "ToastComponent"
Cohesion: 0.31
Nodes (4): ToastComponent, Component, Inject, IToast

### Community 251 - "LanguageService"
Cohesion: 0.04
Nodes (38): SupportLanguages, IEventByIdResult, IEventDropdownListResult, IEventListResult, GameDetailsComponent, Component, ImagePreviewData, ImagePreviewDialog (+30 more)

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

### Community 256 - "EmailHelperService"
Cohesion: 0.28
Nodes (5): Dictionary, IDbContextFactory, Task, EmailHelperService, EmailType

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
Cohesion: 0.06
Nodes (31): DH.OperationResultCore.Extension, CancellationToken, Task, GetReservationChartDataQuery, GetReservationChartDataQueryHandler, GetReservationChartData, ReservationStats, Dictionary (+23 more)

### Community 267 - ".Handle"
Cohesion: 0.36
Nodes (7): CancellationToken, List, Task, GetRoomMemberListQuery, GetRoomMemberListQueryHandler, DateTime, GetRoomMemberListQueryModel

### Community 268 - "DH.Adapter.FileManager"
Cohesion: 0.33
Nodes (6): DH.Adapter.FileManager, net8.0, Microsoft.Extensions.Configuration.Abstractions (8.0.0), Microsoft.NET.Sdk, Supabase (1.1.1), Supabase.Storage (2.4.1)

### Community 269 - "UpdateTenantSettingsCommandHandler"
Cohesion: 0.48
Nodes (5): CancellationToken, ILogger, Task, UpdateTenantSettingsCommand, UpdateTenantSettingsCommandHandler

### Community 270 - "StreakComponent"
Cohesion: 0.29
Nodes (3): StreakComponent, StreakPageType, Component

### Community 271 - "GetUserWhoPlayedGameChartDataQueryHandler"
Cohesion: 0.27
Nodes (8): CancellationToken, Task, GetUserWhoPlayedGameChartDataQuery, GetUserWhoPlayedGameChartDataQueryHandler, DateTime, List, GameUserActivity, GetUsersWhoPlayedGameData

### Community 272 - "TenantDbContextModelSnapshot.cs"
Cohesion: 0.25
Nodes (5): ModelBuilder, TenantDbContextModelSnapshot, ModelBuilder, StatisticsDbContextModelSnapshot, ModelSnapshot

### Community 274 - "GetActiveGameReservationListQueryHandler"
Cohesion: 0.43
Nodes (7): CancellationToken, List, Task, GetActiveGameReservationListQuery, GetActiveGameReservationListQueryHandler, DateTime, GetActiveGameReservationListQueryModel

### Community 275 - "GetAllEventsDropdownListQueryHandler"
Cohesion: 0.50
Nodes (6): CancellationToken, List, Task, GetAllEventsDropdownListModel, GetAllEventsDropdownListQuery, GetAllEventsDropdownListQueryHandler

### Community 276 - "GetEventListForUserQueryHandler"
Cohesion: 0.53
Nodes (5): CancellationToken, List, Task, GetEventListForUserQuery, GetEventListForUserQueryHandler

### Community 277 - "GetActiveSpaceTableReservationCountQueryHandler"
Cohesion: 0.47
Nodes (4): CancellationToken, Task, GetActiveSpaceTableReservationCountQuery, GetActiveSpaceTableReservationCountQueryHandler

### Community 278 - "EventMessage"
Cohesion: 0.12
Nodes (14): DateTimeOffset, EventMessage, CancellationToken, Task, CancellationToken, Task, CancellationToken, Task (+6 more)

### Community 279 - "games-library.component.ts"
Cohesion: 0.05
Nodes (25): IGameCategory, IGameListResult, ICreateSpaceReservation, GameCategoriesComponent, Component, GameNavigationComponent, Component, NewGameListComponent (+17 more)

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
Nodes (5): MigrationBuilder, InitialTenant, MigrationBuilder, InitialCreate, Migration

### Community 292 - "DH.Adapter.Localization"
Cohesion: 0.40
Nodes (5): DH.Adapter.Localization, net8.0, Microsoft.NET.Sdk, Microsoft.AspNetCore.Localization (2.3.0), Microsoft.Extensions.Localization (8.0.19)

### Community 293 - "DH.Adapter.PushNotifications"
Cohesion: 0.40
Nodes (5): DH.Adapter.PushNotifications, net8.0, Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Sdk, FirebaseAdmin (3.0.1)

### Community 294 - "DH.Adapter.Statistics"
Cohesion: 0.40
Nodes (5): DH.Adapter.Statistics, net8.0, Microsoft.Extensions.DependencyInjection.Abstractions (8.0.2), Microsoft.Extensions.Hosting.Abstractions (8.0.1), Microsoft.NET.Sdk

### Community 296 - "ChipComponent"
Cohesion: 0.40
Nodes (3): ChipComponent, Component, Input

### Community 298 - "GetGameByIdQueryHandler"
Cohesion: 0.36
Nodes (6): CancellationToken, Task, GetGameByIdQuery, GetGameByIdQueryHandler, GameAveragePlaytime, GetGameByIdQueryModel

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

### Community 306 - "GetUserStatsQueryHandler"
Cohesion: 0.39
Nodes (5): CancellationToken, Task, GetUserStatsQuery, GetUserStatsQueryHandler, GetUserStatsQueryModel

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

### Community 371 - "PartnerInquiryDto"
Cohesion: 0.33
Nodes (5): int, List, Regex, ValidationError, PartnerInquiryDto

### Community 373 - "DH.Adapter.Reservations"
Cohesion: 0.40
Nodes (3): DH.Adapter.Reservations, IServiceCollection, ReservationAdapterDI

### Community 374 - "CreateGameCommandHandler"
Cohesion: 0.50
Nodes (4): CancellationToken, Task, CreateGameCommand, CreateGameCommandHandler

### Community 375 - "UpdateGameCommandHandler"
Cohesion: 0.50
Nodes (4): CancellationToken, Task, UpdateGameCommand, UpdateGameCommandHandler

### Community 377 - "CreateGameDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, CreateGameDto

### Community 378 - "NavBarComponent"
Cohesion: 0.40
Nodes (3): NavBarComponent, Component, Input

### Community 385 - "UpdateRewardDto"
Cohesion: 0.40
Nodes (4): int, List, ValidationError, UpdateRewardDto

### Community 386 - "CreateRoomCommandDto"
Cohesion: 0.40
Nodes (4): DateTime, List, ValidationError, CreateRoomCommandDto

### Community 387 - "UserSettingsDto"
Cohesion: 0.43
Nodes (6): CancellationToken, Task, GetUserSettingsQuery, GetUserSettingsQueryHandler, bool, UserSettingsDto

### Community 390 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, GameQRCodeState

### Community 391 - ".HandleAsync"
Cohesion: 0.50
Nodes (3): CancellationToken, Task, UnknownQRCodeState

### Community 392 - "ChallengeProcessingOutcomeMessage"
Cohesion: 0.67
Nodes (3): DateTime, ChallengeOutcome, ChallengeProcessingOutcomeMessage

## Knowledge Gaps
- **420 isolated node(s):** `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)`, `xunit.runner.visualstudio (2.8.1)`, `Npgsql (8.0.3)` (+415 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **82 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `DH.Domain.Entities` connect `DH.Domain.Entities` to `TenantDbContext`, `DH.Domain.Adapters.Data`, `DH.Domain.Adapters.FileManager`, `DH.Domain.Adapters.Localization`, `DH.Domain.Models.SpaceManagementModels.Queries`, `DH.OperationResultCore.Exceptions`, `GetAllEventsDropdownListQueryHandler`, `DH.Domain.Repositories`, `ChallengeReward`, `DH.Domain.Adapters.Authentication`, `DH.Domain.Models.Common`, `.GetGlobalTenantSettingsAsync`, `IEmailHelperService`, `UserChallengesManagementService`, `IUserManagementService`, `Tenant`, `UniversalChallengeProcessing`, `ReservationStatus`, `UserChallenge`, `IEventService`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **Why does `ILocalizationService` connect `ILocalizationService` to `UpdateRewardDto`, `CreateRoomCommandDto`, `LocalizationService`, `.HandleAsync`, `ITenantDbContext`, `IValidableFields`, `UpdateTenantSettingsCommandHandler`, `TenantApplicationsController`, `.SendNotificationToUsersAsync`, `.FieldsAreValid`, `GetActiveGameReservationListQueryHandler`, `ChallengesController`, `QRReaderModel`, `ChallengeReward`, `RoomsController`, `IRequestHandler`, `.GetUserLocalOrUtcTime`, `UserManagementService`, `EmployeeService`, `IEmailHelperService`, `OwnerService`, `GameService`, `AuthenticationService`, `GetActiveSpaceTableReservationListQueryHandler`, `UserRegistrationRequest`, `IValidableFields`, `SchedulerService`, `IChallengeService`, `GetSpaceTableParticipantListQueryHandler`, `SendTenantOwnerCredentialsEmailCommandHandler`, `GetRoomInfoMessageListQueryHandler`, `IRequest`, `QrCodeValidationResult`, `SpaceTableService`, `PartnerInquiryDto`, `CreateGameCommandHandler`, `UpdateGameCommandHandler`, `CreateGameDto`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Why does `DH.Domain.Enums` connect `DH.Domain.Entities` to `TenantDbContext`, `DH.Domain.Adapters.Data`, `.GetActivityChartData`, `DH.Domain.Adapters.FileManager`, `DH.Domain.Adapters.Localization`, `DH.Domain.Models.SpaceManagementModels.Queries`, `TenantApplicationsController`, `DH.OperationResultCore.Exceptions`, `DH.Domain.Repositories`, `DH.Domain.Adapters.Authentication`, `DH.Domain.Models.Common`, `StatisticsService`, `TenantApplicationDto`, `ReservationType`, `GetGameByIdQueryHandler`, `IStatisticJobInfo`, `UserChallengesManagementService`, `Tenant`, `IChallengeService`, `UniversalChallengeProcessing`, `QueuedJob`, `ReservationStatus`, `DH.Domain.Models.StatisticsModels.Queries`, `UserChallenge`?**
  _High betweenness centrality (0.026) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Test.Sdk (17.10.0)`, `xunit (2.8.1)` to the rest of the system?**
  _420 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TenantRouter` be split into smaller, more focused modules?**
  _Cohesion score 0.044512402310567446 - nodes in this community are weakly interconnected._
- **Should `SpaceManagementService` be split into smaller, more focused modules?**
  _Cohesion score 0.04719887955182073 - nodes in this community are weakly interconnected._
- **Should `app.module.ts` be split into smaller, more focused modules?**
  _Cohesion score 0.02670856245090338 - nodes in this community are weakly interconnected._