import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { AdminChallengesManagementRoutingModule } from './admin-challenges-management-routes.module';
import { AdminChallengesNavigationComponent } from './page/admin-challenges-navigation.component';
import { AdminChallengesHistoryLogComponent } from '../../features/admin-challenges-management/components/admin-challenges-history-log/admin-challenges-history-log.component';
import { AdminChallengesListComponent } from '../../features/admin-challenges-management/components/admin-challenges-list/admin-challenges-list.component';
import { AdminChallengesRewardsComponent } from '../../features/admin-challenges-management/components/admin-challenges-rewards/admin-challenges-rewards.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { AdminChallengesSystemRewardsComponent } from '../../features/admin-challenges-management/components/admin-challenges-system-rewards/admin-challenges-system-rewards.component';
import { AdminChallengesRewardConfirmDeleteDialog } from '../../features/admin-challenges-management/dialogs/admin-challenges-reward-confirm-delete/admin-challenges-reward-confirm-delete.component';
import { MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { AdminChallengesConfirmDeleteDialog } from '../../features/admin-challenges-management/dialogs/admin-challenges-confirm-delete/admin-challenges-confirm-delete.component';

@NgModule({
  declarations: [
    AdminChallengesNavigationComponent,
    AdminChallengesHistoryLogComponent,
    AdminChallengesListComponent,
    AdminChallengesRewardsComponent,
    AdminChallengesSystemRewardsComponent,
    AdminChallengesRewardConfirmDeleteDialog,
    AdminChallengesConfirmDeleteDialog
  ],
  exports: [],
  providers: [],
  imports: [
    SharedModule,
    HeaderModule,
    MatDialogActions,
    MatDialogClose,
    AdminChallengesManagementRoutingModule,
    NavBarModule,
    NgSelectModule,
    FormsModule,
  ],
})
export class AdminChallengesManagementModule {}
