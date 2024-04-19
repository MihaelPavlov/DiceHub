import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { HeaderModule } from '../../widgets/header/header.module';
import { ChallengesManagementRoutingModule } from './challenges-management-routes.module';
import { ChallengesManagementComponent } from './page/challenges-management.component';
import { NavBarModule } from "../../widgets/nav-bar/nav-bar.module";

@NgModule({
    declarations: [ChallengesManagementComponent],
    exports: [ChallengesManagementComponent],
    providers: [],
    imports: [SharedModule, HeaderModule, ChallengesManagementRoutingModule, NavBarModule]
})
export class ChallengesManagementModule {}
