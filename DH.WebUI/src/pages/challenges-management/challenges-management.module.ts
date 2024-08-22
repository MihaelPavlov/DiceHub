import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChallengesManagementRoutingModule } from './challenges-management-routes.module';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { NavBarModule } from '../../widgets/nav-bar/nav-bar.module';
import { ChallengesRewardsComponent } from '../../features/challenges-management/components/challenges-rewards/challenges-rewards.component';
import { ChipModule } from "../../widgets/chip/chip.module";

@NgModule({
    declarations: [ChallengesManagementComponent, ChallengesRewardsComponent],
    exports: [ChallengesManagementComponent],
    providers: [],
    imports: [
        SharedModule,
        HeaderModule,
        ChallengesManagementRoutingModule,
        NavBarModule,
        ChipModule,
    ]
})
export class ChallengesManagementModule {}
