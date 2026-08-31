# Graph Report - DH.WebUI  (2026-08-22)

## Corpus Check
- 408 files · ~3,371,095 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 2711 nodes · 7862 edges · 178 communities (127 shown, 51 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 3 edges (avg confidence: 0.7)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `e9585879`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- TenantRouter
- add-update-meeple-room.component.ts
- Form
- AdminChallengesCustomPeriodComponent
- .post
- games-library.component.ts
- ToastService
- LinkInfoComponent
- UsersService
- .error
- .get
- games-library.module.ts
- app.module.ts
- statistics.service.ts
- qr-code-scanner.component.ts
- challenges-management.module.ts
- reservation-confirmation.dialog.ts
- app.component.ts
- SupportLanguages
- GameReservations
- RandomColorDirective
- auth.service.ts
- GameNavigationComponent
- games.service.ts
- .navigateTenant
- AdminUniversalChallengesComponent
- NotificationsDialog
- ClubSpaceManagementComponent
- GamesChartComponent
- SpaceTableActiveReservations
- TenantService
- GlobalSettingsComponent
- dependencies
- devDependencies
- ChallengesManagementComponent
- AdminChallengesSystemRewardsComponent
- RegisterChoiceComponent
- ChallengesService
- RoomChatComponent
- SpaceBookingComponent
- charts.module.ts
- HeaderComponent
- challenges-management.component.ts
- GameAvailabilityComponent
- NavigationMenuComponent
- MeepleRoomDetailsComponent
- AddUpdateEventComponent
- AuthService
- AddUpdateGameComponent
- AssistiveTouchComponent
- EventDetailsComponent
- .put
- MessagingService
- LoadingService
- Chart2Component
- RestApiService
- AddUpdateMeepleRoomComponent
- ChallengesRewardsComponent
- UserSettingsComponent
- challenges-management-routes.module.ts
- options
- CreateEmployeePasswordComponent
- CreateOwnerPasswordComponent
- .setPreviousUrl
- VisitorsChartComponent
- ConfirmEmailComponent
- qr-code-scanner.module.ts
- TenantSettingsService
- RoomsService
- EventsLibraryComponent
- rewards.service.ts
- ReservationHistoryActionsComponent
- scripts
- RegisterComponent
- AddUpdateEmployeeComponent
- GameReviewConfirmDeleteDialog
- GameReviewsComponent
- GameReservationHistory
- ChallengeType
- EventsService
- MeepleRoomMenuComponent
- TenantUserSettingsService
- ReservationManagementNavigationComponent
- VenueApplicationComponent
- DiceRollerComponent
- DHWebUI
- SpaceTableReservationHistory
- AddUpdateClubSpaceComponent
- .resetData
- ControlsMenuComponent
- RoomConfirmLeaveDialog
- LoginComponent
- .userinfo
- rest-api.service.ts
- FrontEndLogService
- RewardsCollectedChartComponent
- .buildTenantUrl
- CollectedExpiredRewardsChartComponent
- manifest.json
- language.service.ts
- AuthTokenService
- admin-challenges-custom-period.component.ts
- path.config.ts
- rooms.service.ts
- RoomMembersComponent
- GameLayoutComponent
- forbidden.module.ts
- TenantContextService
- shared.module.ts
- unauthorized.module.ts
- development
- production
- global-settings.component.ts
- IGameByIdResult
- EventAttendanceChartComponent
- AdminEventManagementComponent
- ForgotPasswordComponent
- PasswordVisibilityToggleComponent
- ScrollTopComponent
- DH.WebUI
- ExampleInstrumentedTest.java
- ReservationsChartComponent
- ResetPasswordComponent
- ToastComponent
- angular.json
- architect
- CalculateRemainingDaysPipe
- NavBarComponent
- ExampleUnitTest.java
- gradlew
- assets
- .onUpdate
- AdminChallengesConfirmDeleteDialog
- RoomConfirmDeleteDialog
- GameConfirmDeleteDialog
- ParseDateTagPipe
- add-update-club-space.component.ts
- DiceRollerComponent
- MainActivity.java
- RewardsService
- RoomMemberConfirmDeleteDialog
- EmployeeConfirmDeleteDialog
- IRewardListResult
- @angular/compiler
- AppComponent
- @angular/core
- @angular/forms
- @angular/fire
- @angular/platform-browser-dynamic
- @angular/material
- angularx-qrcode
- chartjs-adapter-date-fns
- chartjs-plugin-datalabels
- @angular/platform-browser
- deploy.sh
- @auth0/angular-jwt
- memoize-one
- @capacitor/android
- capacitor.config.ts
- @ngx-translate/http-loader
- @capacitor-firebase/messaging
- tslib
- challenge-dropdown.model.ts
- game-qr-code.model.ts
- tenant-settings.interface.ts
- environment.prod.ts
- date-fns
- @microsoft/signalr
- .constructor
- IMenuItemInterface
- .closeInteractive

## God Nodes (most connected - your core abstractions)
1. `TenantRouter` - 165 edges
2. `ToastService` - 116 edges
3. `AuthService` - 110 edges
4. `LanguageService` - 94 edges
5. `GamesService` - 77 edges
6. `MenuTabsService` - 76 edges
7. `SupportLanguages` - 60 edges
8. `SpaceManagementService` - 56 edges
9. `Form` - 55 edges
10. `ToastType` - 54 edges

## Surprising Connections (you probably didn't know these)
- `AppComponent` --references--> `IUserInfo`  [EXTRACTED]
  src/app/app-component/app.component.ts → src/entities/auth/models/user-info.model.ts
- `AppComponent` --references--> `ChallengeOverlayComponent`  [EXTRACTED]
  src/app/app-component/app.component.ts → src/shared/components/challenge-overlay/challenge-overlay.component.ts
- `AuthService` --references--> `IUserInfo`  [EXTRACTED]
  src/entities/auth/auth.service.ts → src/entities/auth/models/user-info.model.ts
- `GameReviewsComponent` --references--> `IUserInfo`  [EXTRACTED]
  src/features/games-library/components/game-reviews/page/game-reviews.component.ts → src/entities/auth/models/user-info.model.ts
- `AdminChallengesListComponent` --references--> `IChallengeListResult`  [EXTRACTED]
  src/features/challenges-management/components/admin-challenges-list/admin-challenges-list.component.ts → src/entities/challenges/models/challenge-list.model.ts

## Import Cycles
- None detected.

## Communities (178 total, 51 thin omitted)

### Community 0 - "TenantRouter"
Cohesion: 0.10
Nodes (12): GamesService, Injectable, TenantRouter, Injectable, LanguageService, Injectable, MenuTabsService, Injectable (+4 more)

### Community 1 - "add-update-meeple-room.component.ts"
Cohesion: 0.14
Nodes (12): IRoomByIdResult, futureDateValidator(), IAddUpdateRoomForm, routes, ImagePreviewData, ImagePreviewDialog, Component, Inject (+4 more)

### Community 2 - "Form"
Cohesion: 0.18
Nodes (4): ChangePasswordComponent, Component, Form, Component

### Community 3 - "AdminChallengesCustomPeriodComponent"
Cohesion: 0.07
Nodes (4): ICustomPeriod, AdminChallengesCustomPeriodComponent, customPeriodValidator(), Component

### Community 4 - ".post"
Cohesion: 0.06
Nodes (20): TenantApplicationsService, Injectable, ICompleteTenantSetupRequest, ICompleteTenantSetupResult, ISeedGameCatalogDropdown, ITenantApplication, ITenantApplicationRequest, ITenantApplicationReviewRequest (+12 more)

### Community 5 - "games-library.component.ts"
Cohesion: 0.09
Nodes (18): GameCategoriesService, Injectable, IGameCategory, IGameListResult, ICreateGameForm, NewGameListComponent, Component, QrCodeDialog (+10 more)

### Community 6 - "ToastService"
Cohesion: 0.15
Nodes (21): ISystemRewardsForm, ICreateSpaceReservation, ICreateEventForm, IChangePasswordForm, IUserSettingsForm, ICreateEmployeePasswordForm, ICreateOwnerPasswordForm, IForgotPasswordForm (+13 more)

### Community 7 - "LinkInfoComponent"
Cohesion: 0.07
Nodes (18): INSTRUCTION_LINK_MAPPINGS, InstructionSection, InstructionStep, InstructionTopic, LinkInfoType, StepActionLink, InstructionComponent, Component (+10 more)

### Community 8 - "UsersService"
Cohesion: 0.11
Nodes (8): Injectable, UsersService, GetOwnerStats, GetUserStats, IOwnerResult, IUser, OwnerDetailsComponent, Component

### Community 10 - ".get"
Cohesion: 0.06
Nodes (13): SpaceManagementService, Injectable, ISpaceTableById, ISpaceTableParticipant, ClubSpaceDetailsComponent, Component, JoinTableConfirmDialog, JoinTableConfirmDialogData (+5 more)

### Community 11 - "games-library.module.ts"
Cohesion: 0.08
Nodes (27): ClubSpaceManagementModule, NgModule, ClubSpaceManagementRoutingModule, NgModule, EventsLibraryModule, NgModule, EventsLibraryRoutingModule, NgModule (+19 more)

### Community 12 - "app.module.ts"
Cohesion: 0.07
Nodes (25): AppRoutingModule, NgModule, ROUTES, ConfirmEmailModule, NgModule, CreateEmployeePasswordModule, NgModule, CreateOwnerPasswordModule (+17 more)

### Community 13 - "statistics.service.ts"
Cohesion: 0.09
Nodes (23): StatisticsService, Injectable, ChartActivityType, GamesActivityType, ActivityLog, GetActivityChartData, IChallengeLeaderboard, GetCollectedRewardsByDates (+15 more)

### Community 14 - "qr-code-scanner.component.ts"
Cohesion: 0.06
Nodes (24): ScannerService, Injectable, QrCodeType, IQrCode, IQrCodeRequest, IQrCodeValidationResult, Component, Inject (+16 more)

### Community 15 - "challenges-management.module.ts"
Cohesion: 0.07
Nodes (17): AdminChallengesHistoryLogComponent, Component, StreakLeaderboardComponent, Component, StreakRewardsComponent, Component, StreakComponent, StreakPageType (+9 more)

### Community 16 - "reservation-confirmation.dialog.ts"
Cohesion: 0.16
Nodes (15): ReservationDetailsActions, ReservationConfirmation, IReservationConfirmationForm, ReservationDetailsDialog, Component, ReservationConfirmationDialog, Component, Inject (+7 more)

### Community 17 - "app.component.ts"
Cohesion: 0.12
Nodes (7): TODO: Check this tread…, ChallengeHubService, Injectable, ChallengeOverlayComponent, Component, ChallengeOverlayService, Injectable

### Community 19 - "GameReservations"
Cohesion: 0.15
Nodes (3): IReservedGame, GameReservations, Component

### Community 20 - "RandomColorDirective"
Cohesion: 0.33
Nodes (3): Directive, RandomColorDirective, Input

### Community 21 - "auth.service.ts"
Cohesion: 0.17
Nodes (9): UserRole, IRegisterRequest, IRegisterResponse, ITokenResponse, IUserInfo, ReviewState, IEmployeeForm, routes (+1 more)

### Community 23 - "games.service.ts"
Cohesion: 0.12
Nodes (20): ActiveReservedGame, IGameInventory, IGameReservationStatus, IGetReservationById, ActiveBookedTableModel, getKeyFriendlyNames(), IActiveReservedTable, IAddSpaceTableDto (+12 more)

### Community 24 - ".navigateTenant"
Cohesion: 0.09
Nodes (6): EventsChartsLayoutComponent, Component, RewardChartsLayoutComponent, Component, ProfileComponent, Component

### Community 26 - "NotificationsDialog"
Cohesion: 0.12
Nodes (7): NotificationsService, Injectable, IUserNotification, NotificationsDialog, Component, Inject, ViewChild

### Community 27 - "ClubSpaceManagementComponent"
Cohesion: 0.10
Nodes (6): ISpaceTableList, ClubSpaceListComponent, Component, routes, ClubSpaceManagementComponent, Component

### Community 30 - "TenantService"
Cohesion: 0.09
Nodes (13): ITenantListResult, SuperadminTenantDetailsComponent, Component, SuperadminTenantsComponent, Component, SelectClubComponent, Component, SelectClubModule (+5 more)

### Community 31 - "GlobalSettingsComponent"
Cohesion: 0.13
Nodes (6): IUserSettings, GlobalSettingsComponent, Component, UiTheme, ThemeService, Injectable

### Community 32 - "dependencies"
Cohesion: 0.07
Nodes (27): @angular/animations, @angular/common, @angular/router, @capacitor/app, @capacitor/core, chart.js, crypto-js, firebase (+19 more)

### Community 33 - "devDependencies"
Cohesion: 0.07
Nodes (27): @angular/compiler-cli, @angular-devkit/build-angular, @capacitor/cli, gifsicle, jasmine-core, karma, karma-chrome-launcher, karma-coverage (+19 more)

### Community 34 - "ChallengesManagementComponent"
Cohesion: 0.11
Nodes (6): IUserCustomPeriodChallenge, IUserChallengePeriodReward, ChallengesManagementComponent, Component, ViewChild, ViewChildren

### Community 35 - "AdminChallengesSystemRewardsComponent"
Cohesion: 0.12
Nodes (4): AdminChallengesSystemRewardsComponent, Component, EntityImagePipe, Pipe

### Community 36 - "RegisterChoiceComponent"
Cohesion: 0.20
Nodes (4): RegisterChoiceComponent, Component, RedirectIfTenantGuard, Injectable

### Community 37 - "ChallengesService"
Cohesion: 0.12
Nodes (6): ChallengesService, Injectable, AdminChallengesListComponent, Component, ScrollService, Injectable

### Community 38 - "RoomChatComponent"
Cohesion: 0.16
Nodes (6): IRoomInfoMessageResult, GroupedChatMessage, IGroupMessage, RoomChatComponent, Component, ViewChild

### Community 39 - "SpaceBookingComponent"
Cohesion: 0.14
Nodes (3): SpaceBookingComponent, Component, ViewChild

### Community 40 - "charts.module.ts"
Cohesion: 0.13
Nodes (9): ChallengeLeaderboardType, IChallengeLeaderboardData, LeaderboardChallengesComponent, Component, ChartAppModule, NgModule, ChartRoutingModule, routes (+1 more)

### Community 41 - "HeaderComponent"
Cohesion: 0.12
Nodes (4): HeaderComponent, Component, Input, Output

### Community 42 - "challenges-management.component.ts"
Cohesion: 0.22
Nodes (14): ChallengeRewardPoint, ChallengeStatus, IChallengeResult, IChallengeListResult, ICreateChallengeDto, IUniversalChallengeListResult, IUpdateChallengeDto, IUpdateUniversalChallengeDto (+6 more)

### Community 43 - "GameAvailabilityComponent"
Cohesion: 0.12
Nodes (3): ICreateGameReservation, GameAvailabilityComponent, Component

### Community 45 - "MeepleRoomDetailsComponent"
Cohesion: 0.19
Nodes (3): MeepleRoomDetailsComponent, Component, ViewChild

### Community 46 - "AddUpdateEventComponent"
Cohesion: 0.12
Nodes (6): AddUpdateEventComponent, futureDateValidator(), isFutureDate(), parseDateInput(), Component, ViewChild

### Community 47 - "AuthService"
Cohesion: 0.07
Nodes (13): AuthService, Injectable, IStreakLeaderboardData, SettingsOwnerAccessGuard, Injectable, SettingsSuperAdminAccessGuard, Injectable, SettingsUserAccessGuard (+5 more)

### Community 49 - "AddUpdateGameComponent"
Cohesion: 0.12
Nodes (3): AddUpdateGameComponent, Component, ViewChild

### Community 50 - "AssistiveTouchComponent"
Cohesion: 0.18
Nodes (5): AssistiveTouchComponent, Component, HostListener, Input, Output

### Community 51 - "EventDetailsComponent"
Cohesion: 0.10
Nodes (4): AdminEventDetailsComponent, Component, EventDetailsComponent, Component

### Community 53 - "MessagingService"
Cohesion: 0.12
Nodes (7): MessagingService, Injectable, SchedulerService, Injectable, IScheduleJobInfo, JobsComponent, Component

### Community 54 - "LoadingService"
Cohesion: 0.17
Nodes (10): LoadingIndicatorComponent, Component, ContentChild, Input, LoadingInterceptor, Injectable, LoadingInterceptorContextService, Injectable (+2 more)

### Community 55 - "Chart2Component"
Cohesion: 0.20
Nodes (3): Chart2Component, Component, ViewChild

### Community 57 - "AddUpdateMeepleRoomComponent"
Cohesion: 0.14
Nodes (3): IGameDropdownResult, AddUpdateMeepleRoomComponent, Component

### Community 58 - "ChallengesRewardsComponent"
Cohesion: 0.22
Nodes (4): IUserReward, UserRewardStatus, ChallengesRewardsComponent, Component

### Community 59 - "UserSettingsComponent"
Cohesion: 0.18
Nodes (4): Component, UserSettingsComponent, TranslateInPipe, Pipe

### Community 60 - "challenges-management-routes.module.ts"
Cohesion: 0.14
Nodes (8): AdminChallengesNavigationComponent, Component, ChallengesManagementRoutingModule, routes, NgModule, CanComponentDeactivate, canDeactivateGuard(), Column

### Community 61 - "options"
Cohesion: 0.19
Nodes (14): options, baseHref, browser, index, inlineStyleLanguage, outputPath, polyfills, scripts (+6 more)

### Community 62 - "CreateEmployeePasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateEmployeePasswordRequest, CreateEmployeePasswordComponent, Component

### Community 63 - "CreateOwnerPasswordComponent"
Cohesion: 0.16
Nodes (3): ICreateOwnerPasswordRequest, CreateOwnerPasswordComponent, Component

### Community 64 - ".setPreviousUrl"
Cohesion: 0.10
Nodes (4): EmployeeListComponent, Component, InstructionManagementComponent, Component

### Community 65 - "VisitorsChartComponent"
Cohesion: 0.26
Nodes (3): Component, ViewChild, VisitorsChartComponent

### Community 66 - "ConfirmEmailComponent"
Cohesion: 0.14
Nodes (4): ConfirmEmailComponent, Component, LanguageSwitchComponent, Component

### Community 67 - "qr-code-scanner.module.ts"
Cohesion: 0.25
Nodes (7): QrCodeScannerModule, NgModule, QrCodeScannerRoutingModule, routes, NgModule, ScanResultAdminDialogModule, NgModule

### Community 68 - "TenantSettingsService"
Cohesion: 0.22
Nodes (6): TenantSettingsService, Injectable, GetClubInfoModel, ClubInfo, Component, IResetPasswordForm

### Community 70 - "EventsLibraryComponent"
Cohesion: 0.22
Nodes (3): IEventListResult, EventsLibraryComponent, Component

### Community 71 - "rewards.service.ts"
Cohesion: 0.35
Nodes (7): RewardLevel, REWARD_POINTS, RewardRequiredPoint, ICreateRewardDto, IRewardGetByIdResult, IRewardDropdownResult, IUpdateRewardDto

### Community 72 - "ReservationHistoryActionsComponent"
Cohesion: 0.14
Nodes (5): ReservationHistoryActionsComponent, Component, ContentChild, Input, Output

### Community 73 - "scripts"
Cohesion: 0.13
Nodes (14): name, private, scripts, build, cap:open, cap:sync, ng, prod-build (+6 more)

### Community 76 - "GameReviewConfirmDeleteDialog"
Cohesion: 0.33
Nodes (3): GameReviewConfirmDeleteDialog, Component, Inject

### Community 77 - "GameReviewsComponent"
Cohesion: 0.14
Nodes (7): GameReviewsService, Injectable, IGameCreateDto, IGameReviewListResult, IGameUpdateDto, GameReviewsComponent, Component

### Community 78 - "GameReservationHistory"
Cohesion: 0.35
Nodes (3): IGameReservationHistory, GameReservationHistory, Component

### Community 79 - "ChallengeType"
Cohesion: 0.23
Nodes (7): AdminChallengesComponent, Component, ChallengeType, ChallengeTypeToggleComponent, Component, Input, Output

### Community 80 - "EventsService"
Cohesion: 0.11
Nodes (10): EventsService, Injectable, ICreateEventDto, IEventDropdownListResult, EventAttendanceByEventsChartComponent, Component, ViewChild, EventConfirmDeleteDialog (+2 more)

### Community 81 - "MeepleRoomMenuComponent"
Cohesion: 0.23
Nodes (5): MeepleRoomMenuComponent, Component, HostListener, Input, Output

### Community 82 - "TenantUserSettingsService"
Cohesion: 0.42
Nodes (3): TenantUserSettingsService, Injectable, AssistiveTouchSettings

### Community 83 - "ReservationManagementNavigationComponent"
Cohesion: 0.15
Nodes (3): ReservationManagementNavigationComponent, Component, ViewChild

### Community 85 - "DiceRollerComponent"
Cohesion: 0.22
Nodes (4): DiceRollerComponent, Component, Input, Output

### Community 86 - "DHWebUI"
Cohesion: 0.25
Nodes (7): Build, Code scaffolding, Development server, DHWebUI, Further help, Running end-to-end tests, Running unit tests

### Community 87 - "SpaceTableReservationHistory"
Cohesion: 0.31
Nodes (3): ITableReservationHistory, SpaceTableReservationHistory, Component

### Community 88 - "AddUpdateClubSpaceComponent"
Cohesion: 0.10
Nodes (4): AddUpdateClubSpaceComponent, Component, GameDetailsComponent, Component

### Community 89 - ".resetData"
Cohesion: 0.11
Nodes (4): GameCategoriesComponent, Component, FindMeepleManagementComponent, Component

### Community 90 - "ControlsMenuComponent"
Cohesion: 0.22
Nodes (4): ControlsMenuComponent, Component, Input, Output

### Community 91 - "RoomConfirmLeaveDialog"
Cohesion: 0.33
Nodes (3): RoomConfirmLeaveDialog, Component, Inject

### Community 93 - ".userinfo"
Cohesion: 0.25
Nodes (5): initializeUserFactory(), ChallengeAdminAccessGuard, Injectable, ChallengeUserAccessGuard, Injectable

### Community 94 - "rest-api.service.ts"
Cohesion: 0.28
Nodes (6): AppModule, NgModule, environment, ApiBase, ApiConfig, ApiEndpoints

### Community 95 - "FrontEndLogService"
Cohesion: 0.24
Nodes (4): GlobalErrorHandler, Injectable, FrontEndLogService, Injectable

### Community 96 - "RewardsCollectedChartComponent"
Cohesion: 0.27
Nodes (3): RewardsCollectedChartComponent, Component, ViewChild

### Community 98 - "CollectedExpiredRewardsChartComponent"
Cohesion: 0.25
Nodes (3): CollectedExpiredRewardsChartComponent, Component, ViewChild

### Community 99 - "manifest.json"
Cohesion: 0.18
Nodes (10): background_color, description, display, icons, name, orientation, scope, short_name (+2 more)

### Community 100 - "language.service.ts"
Cohesion: 0.25
Nodes (4): IEventByIdResult, IOwnerForm, routes, NAV_ITEM_LABELS

### Community 101 - "AuthTokenService"
Cohesion: 0.29
Nodes (4): HttpRequestInterceptor, Injectable, AuthTokenService, Injectable

### Community 102 - "admin-challenges-custom-period.component.ts"
Cohesion: 0.13
Nodes (15): ICustomPeriodChallenge, ICustomPeriodReward, ICustomPeriodUniversalChallenge, IUserChallengePeriodPerformance, TimePeriodType, ITenantSettings, IUniversalChallengeDropdownResult, ICustomPeriodForm (+7 more)

### Community 103 - "path.config.ts"
Cohesion: 0.14
Nodes (6): PartnerInquiriesService, Injectable, IPartnerInquiryRequest, LandingComponent, Component, PATH

### Community 104 - "rooms.service.ts"
Cohesion: 0.21
Nodes (3): IAddUpdateRoomDto, IRoomListResult, IRoomMessageResult

### Community 105 - "RoomMembersComponent"
Cohesion: 0.25
Nodes (3): IRoomMemberResult, RoomMembersComponent, Component

### Community 106 - "GameLayoutComponent"
Cohesion: 0.18
Nodes (5): GameLayoutComponent, Component, Input, Output, NavItemInterface

### Community 107 - "forbidden.module.ts"
Cohesion: 0.33
Nodes (5): ForbiddenModule, NgModule, ForbiddenRoutingModule, routes, NgModule

### Community 108 - "TenantContextService"
Cohesion: 0.08
Nodes (17): TenantLayoutComponent, Component, ExceptionBaseComponent, ForbiddenComponent, Component, NotFoundComponent, Component, NotFoundModule (+9 more)

### Community 109 - "shared.module.ts"
Cohesion: 0.14
Nodes (13): InstructionManagementModule, NgModule, LanguageSwitchModule, NgModule, ServerErrorModule, NgModule, ServerErrorRoutingModule, NgModule (+5 more)

### Community 110 - "unauthorized.module.ts"
Cohesion: 0.28
Nodes (7): Component, UnauthorizedComponent, NgModule, UnauthorizedModule, routes, NgModule, UnauthorizedRoutingModule

### Community 111 - "development"
Cohesion: 0.22
Nodes (9): build, builder, configurations, defaultConfiguration, development, buildTarget, extractLicenses, optimization (+1 more)

### Community 112 - "production"
Cohesion: 0.22
Nodes (9): serve, production, budgets, buildTarget, fileReplacements, outputHashing, builder, configurations (+1 more)

### Community 113 - "global-settings.component.ts"
Cohesion: 0.47
Nodes (3): ToggleState, ITenantSettingsForm, IDropdown

### Community 114 - "IGameByIdResult"
Cohesion: 0.46
Nodes (4): GameAveragePlaytime, ICreateGameDto, IGameByIdResult, IUpdateGameDto

### Community 115 - "EventAttendanceChartComponent"
Cohesion: 0.24
Nodes (3): EventAttendanceChartComponent, Component, ViewChild

### Community 118 - "PasswordVisibilityToggleComponent"
Cohesion: 0.33
Nodes (4): PasswordVisibilityToggleComponent, Component, Input, Output

### Community 119 - "ScrollTopComponent"
Cohesion: 0.25
Nodes (5): ScrollTopComponent, Component, HostListener, ScrollToTopModule, NgModule

### Community 120 - "DH.WebUI"
Cohesion: 0.25
Nodes (8): prefix, projectType, root, schematics, sourceRoot, DH.WebUI, style, @schematics/angular:component

### Community 121 - "ExampleInstrumentedTest.java"
Cohesion: 0.60
Nodes (3): ExampleInstrumentedTest, Test, RunWith

### Community 122 - "ReservationsChartComponent"
Cohesion: 0.27
Nodes (3): ReservationsChartComponent, Component, ViewChild

### Community 123 - "ResetPasswordComponent"
Cohesion: 0.20
Nodes (3): IResetPasswordRequest, ResetPasswordComponent, Component

### Community 124 - "ToastComponent"
Cohesion: 0.36
Nodes (3): ToastComponent, Component, Inject

### Community 125 - "angular.json"
Cohesion: 0.29
Nodes (6): analytics, cli, newProjectRoot, projects, $schema, version

### Community 126 - "architect"
Cohesion: 0.29
Nodes (7): extract-i18n, test, architect, builder, options, buildTarget, builder

### Community 128 - "NavBarComponent"
Cohesion: 0.40
Nodes (3): NavBarComponent, Component, Input

### Community 130 - "gradlew"
Cohesion: 0.83
Nodes (3): gradlew script, die(), warn()

### Community 131 - "assets"
Cohesion: 0.33
Nodes (6): assets, src/favicon.ico, src/firebase-messaging-sw.js, src/manifest.json, src/shared/assets, src/shared/assets/images

### Community 133 - "AdminChallengesConfirmDeleteDialog"
Cohesion: 0.50
Nodes (3): AdminChallengesConfirmDeleteDialog, Component, Inject

### Community 134 - "RoomConfirmDeleteDialog"
Cohesion: 0.50
Nodes (3): RoomConfirmDeleteDialog, Component, Inject

### Community 135 - "GameConfirmDeleteDialog"
Cohesion: 0.50
Nodes (3): GameConfirmDeleteDialog, Component, Inject

### Community 137 - "add-update-club-space.component.ts"
Cohesion: 0.29
Nodes (4): ICreateSpaceTableForm, SinglePlayerConfirmDialog, Component, Inject

### Community 140 - "RewardsService"
Cohesion: 0.20
Nodes (5): RewardsService, Injectable, AdminChallengesRewardConfirmDeleteDialog, Component, Inject

### Community 141 - "RoomMemberConfirmDeleteDialog"
Cohesion: 0.33
Nodes (3): RoomMemberConfirmDeleteDialog, Component, Inject

### Community 142 - "EmployeeConfirmDeleteDialog"
Cohesion: 0.50
Nodes (3): EmployeeConfirmDeleteDialog, Component, Inject

### Community 145 - "AppComponent"
Cohesion: 0.18
Nodes (5): AppComponent, Component, ViewChild, app, messaging

## Knowledge Gaps
- **171 isolated node(s):** `$schema`, `version`, `newProjectRoot`, `projectType`, `style` (+166 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **51 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TenantRouter` connect `TenantRouter` to `NavBarComponent`, `add-update-meeple-room.component.ts`, `.post`, `games-library.component.ts`, `ToastService`, `UsersService`, `add-update-club-space.component.ts`, `.get`, `statistics.service.ts`, `qr-code-scanner.component.ts`, `reservation-confirmation.dialog.ts`, `auth.service.ts`, `games.service.ts`, `.navigateTenant`, `GamesChartComponent`, `TenantService`, `GlobalSettingsComponent`, `SpaceBookingComponent`, `charts.module.ts`, `HeaderComponent`, `challenges-management.component.ts`, `AuthService`, `UserSettingsComponent`, `challenges-management-routes.module.ts`, `CreateEmployeePasswordComponent`, `CreateOwnerPasswordComponent`, `VisitorsChartComponent`, `TenantSettingsService`, `RegisterComponent`, `AddUpdateEmployeeComponent`, `EventsService`, `AddUpdateClubSpaceComponent`, `LoginComponent`, `.buildTenantUrl`, `language.service.ts`, `TenantContextService`, `global-settings.component.ts`, `EventAttendanceChartComponent`, `ForgotPasswordComponent`, `ReservationsChartComponent`, `ResetPasswordComponent`?**
  _High betweenness centrality (0.090) - this node is a cross-community bridge._
- **Why does `ToastService` connect `ToastService` to `TenantRouter`, `add-update-meeple-room.component.ts`, `Form`, `AdminChallengesCustomPeriodComponent`, `AdminChallengesConfirmDeleteDialog`, `RoomConfirmDeleteDialog`, `games-library.component.ts`, `GameConfirmDeleteDialog`, `add-update-club-space.component.ts`, `UsersService`, `.get`, `RewardsService`, `statistics.service.ts`, `RoomMemberConfirmDeleteDialog`, `EmployeeConfirmDeleteDialog`, `reservation-confirmation.dialog.ts`, `qr-code-scanner.component.ts`, `.error`, `auth.service.ts`, `games.service.ts`, `AdminUniversalChallengesComponent`, `GamesChartComponent`, `GlobalSettingsComponent`, `AdminChallengesSystemRewardsComponent`, `ChallengesService`, `SpaceBookingComponent`, `charts.module.ts`, `.constructor`, `challenges-management.component.ts`, `UserSettingsComponent`, `CreateEmployeePasswordComponent`, `CreateOwnerPasswordComponent`, `VisitorsChartComponent`, `TenantSettingsService`, `RegisterComponent`, `AddUpdateEmployeeComponent`, `GameReviewConfirmDeleteDialog`, `EventsService`, `VenueApplicationComponent`, `AddUpdateClubSpaceComponent`, `RoomConfirmLeaveDialog`, `LoginComponent`, `FrontEndLogService`, `language.service.ts`, `admin-challenges-custom-period.component.ts`, `path.config.ts`, `global-settings.component.ts`, `EventAttendanceChartComponent`, `ForgotPasswordComponent`, `ReservationsChartComponent`, `ResetPasswordComponent`?**
  _High betweenness centrality (0.045) - this node is a cross-community bridge._
- **Why does `RestApiService` connect `RestApiService` to `.post`, `games-library.component.ts`, `UsersService`, `.error`, `.get`, `RewardsService`, `statistics.service.ts`, `qr-code-scanner.component.ts`, `auth.service.ts`, `games.service.ts`, `NotificationsDialog`, `TenantService`, `ChallengesService`, `challenges-management.component.ts`, `AuthService`, `.put`, `MessagingService`, `TenantSettingsService`, `RoomsService`, `rewards.service.ts`, `GameReviewsComponent`, `EventsService`, `TenantUserSettingsService`, `rest-api.service.ts`, `FrontEndLogService`, `path.config.ts`, `rooms.service.ts`, `TenantContextService`?**
  _High betweenness centrality (0.038) - this node is a cross-community bridge._
- **What connects `$schema`, `version`, `newProjectRoot` to the rest of the system?**
  _171 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TenantRouter` be split into smaller, more focused modules?**
  _Cohesion score 0.1 - nodes in this community are weakly interconnected._
- **Should `add-update-meeple-room.component.ts` be split into smaller, more focused modules?**
  _Cohesion score 0.13978494623655913 - nodes in this community are weakly interconnected._
- **Should `AdminChallengesCustomPeriodComponent` be split into smaller, more focused modules?**
  _Cohesion score 0.06976744186046512 - nodes in this community are weakly interconnected._