import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChallengesManagementRoutingModule } from './challenges-management-routes.module';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { ChipModule } from '../../widgets/chip/chip.module';
import { QRCodeModule } from 'angularx-qrcode';
import { UserRewardQrCodeDialog } from '../../features/challenges-management/dialogs/user-reward-qr-code-dialog/user-reward-qr-code.component';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { AdminChallengesConfirmDeleteDialog } from '../../features/challenges-management/dialogs/admin-challenges-confirm-delete/admin-challenges-confirm-delete.component';
import { AdminChallengesHistoryLogComponent } from '../../features/challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { AdminChallengesListComponent } from '../../features/challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesCustomPeriodComponent } from '../../features/challenges-management/components/admin-challenges-custom-period/admin-challenges-custom-period.component';
import { AdminChallengesSystemRewardsComponent } from '../../features/challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesRewardConfirmDeleteDialog } from '../../features/challenges-management/dialogs/admin-challenges-reward-confirm-delete/admin-challenges-reward-confirm-delete.component';
import { AdminChallengesNavigationComponent } from './admin-page/admin-challenges-navigation.component';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [
    ChallengesManagementComponent,
    ChallengesRewardsComponent,
    AdminChallengesNavigationComponent,
    AdminChallengesHistoryLogComponent,
    AdminChallengesListComponent,
    AdminChallengesCustomPeriodComponent,
    AdminChallengesSystemRewardsComponent,
    AdminChallengesRewardConfirmDeleteDialog,
    AdminChallengesConfirmDeleteDialog,
    UserRewardQrCodeDialog,
  ],
  exports: [ChallengesManagementComponent],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    ChallengesManagementRoutingModule,
    NavBarModule,
    ChipModule,
    QRCodeModule,
    MatDialogActions,
    MatDialogClose,
    NgSelectModule,
    FormsModule,
  ],
})
export class ChallengesManagementModule {}
