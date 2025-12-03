import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChallengesManagementRoutingModule } from './challenges-management-routes.module';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { UserRewardQrCodeDialog } from '../../features/challenges-management/dialogs/user-reward-qr-code-dialog/user-reward-qr-code.component';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
} from '@angular/material/dialog';
import { AdminChallengesConfirmDeleteDialog } from '../../features/challenges-management/dialogs/admin-challenges-confirm-delete/admin-challenges-confirm-delete.component';
import { AdminChallengesHistoryLogComponent } from '../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { AdminChallengesListComponent } from '../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesCustomPeriodComponent } from '../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesRewardConfirmDeleteDialog } from '../../features/challenges-management/dialogs/admin-challenges-reward-confirm-delete/admin-challenges-reward-confirm-delete.component';
import { AdminChallengesNavigationComponent } from './admin-page/admin-challenges-navigation.component';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { CustomPeriodLeaveConfirmationDialog } from '../../features/challenges-management/dialogs/custom-period-leave-confirmation/custom-period-leave-confirmation.component';
import { UnsavedChangesConfirmationDialogModule } from '../../shared/dialogs/unsaved-changes-confirmation/unsaved-changes-confirmation.module';
import { ImagePreviewDialogModule } from '../../shared/dialogs/image-preview/image-preview.module';
import { ChallengeTypeToggleComponent } from './shared/challenge-type-toggle/challenge-type-toggle.component';
import { StreakLeaderboardComponent } from '../../features/challenges-management/components/streak-leaderboard/streak-leaderboard.component';
import { StreakRewardsComponent } from '../../features/challenges-management/components/streak-reward/streak-rewards.component';
import { StreakComponent } from '../../features/challenges-management/components/streak/streak.component';
import { AdminChallengesComponent } from '../../features/challenges-management/components/admin-challenges/admin-challenges.component';
import { AdminUniversalChallengesComponent } from '../../features/challenges-management/components/admin-universal-challenges/admin-universal-challenges.component';
import { QRCodeComponent } from 'angularx-qrcode';

@NgModule({
  declarations: [
    ChallengesManagementComponent,
    ChallengesRewardsComponent,
    AdminChallengesNavigationComponent,
    AdminChallengesHistoryLogComponent,
    AdminChallengesComponent,
    AdminChallengesListComponent,
    AdminUniversalChallengesComponent,
    AdminChallengesCustomPeriodComponent,
    AdminChallengesSystemRewardsComponent,
    AdminChallengesRewardConfirmDeleteDialog,
    AdminChallengesConfirmDeleteDialog,
    UserRewardQrCodeDialog,
    CustomPeriodLeaveConfirmationDialog,
    ChallengeTypeToggleComponent,
    StreakComponent,
    StreakLeaderboardComponent,
    StreakRewardsComponent
  ],
  exports: [ChallengesManagementComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ChallengesManagementRoutingModule,
    NavBarModule,
    ChipModule,
    QRCodeComponent,
    MatDialogActions,
    MatDialogContent,
    MatDialogClose,
    NgSelectModule,
    FormsModule,
    UnsavedChangesConfirmationDialogModule,
    ImagePreviewDialogModule,
  ],
})
export class ChallengesManagementModule {}
