import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationService } from '../../../shared/services/navigation-service';
import { AuthService } from '../../../entities/auth/auth.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { TranslateService } from '@ngx-translate/core';
import { TenantRouter } from '../../../shared/helpers/tenant-router';
import { INSTRUCTION_LINK_MAPPINGS } from '../../../entities/instruction-management/constants/instruction.constant';

interface InstructionHubLink {
  name: string;
  summary: string;
  path: string;
  icon:
    | 'rocket'
    | 'install'
    | 'bell'
    | 'table'
    | 'calendar'
    | 'trophy'
    | 'meeple';
  accent: 'amber' | 'coral' | 'teal' | 'violet';
  count: number;
}

@Component({
  selector: 'app-instruction-management',
  templateUrl: 'instruction-management.component.html',
  styleUrl: 'instruction-management.component.scss',
  standalone: false,
})
export class InstructionManagementComponent {
  public defaultLinks: InstructionHubLink[] = [];

  constructor(
    private readonly router: Router,
    private readonly tenantRouter: TenantRouter,
    private readonly navigationService: NavigationService,
    private readonly authService: AuthService,
    private readonly ts: TranslateService
  ) {
    this.defaultLinks = [
      {
        name: this.ts.instant('instruction.owner_setup.title'),
        summary: this.ts.instant('instruction.owner_setup.summary'),
        path: '/instructions/owner_setup',
        icon: 'rocket',
        accent: 'violet',
        count: INSTRUCTION_LINK_MAPPINGS['owner_setup'].topics.length,
      },
      {
        name: this.ts.instant('instruction.how_to_install.title'),
        summary: this.ts.instant('instruction.how_to_install.summary'),
        path: '/instructions/how_to_install',
        icon: 'install',
        accent: 'amber',
        count: INSTRUCTION_LINK_MAPPINGS['how_to_install'].topics.length,
      },
      {
        name: this.ts.instant('instruction.notifications.title'),
        summary: this.ts.instant('instruction.notifications.summary'),
        path: '/instructions/notifications',
        icon: 'bell',
        accent: 'coral',
        count: INSTRUCTION_LINK_MAPPINGS['notifications'].topics.length,
      },
      {
        name: this.ts.instant('instruction.reservation.title'),
        summary: this.ts.instant('instruction.reservation.summary'),
        path: '/instructions/reservation',
        icon: 'table',
        accent: 'teal',
        count: INSTRUCTION_LINK_MAPPINGS['reservation'].topics.length,
      },
      {
        name: this.ts.instant('instruction.events.title'),
        summary: this.ts.instant('instruction.events.summary'),
        path: '/instructions/events',
        icon: 'calendar',
        accent: 'violet',
        count: INSTRUCTION_LINK_MAPPINGS['events'].topics.length,
      },
      {
        name: this.ts.instant('instruction.challenges.title'),
        summary: this.ts.instant('instruction.challenges.summary'),
        path: '/instructions/challenges',
        icon: 'trophy',
        accent: 'amber',
        count: INSTRUCTION_LINK_MAPPINGS['challenges'].topics.length,
      },
      {
        name: this.ts.instant('instruction.meeples.title'),
        summary: this.ts.instant('instruction.meeples.summary'),
        path: '/instructions/meeples',
        icon: 'meeple',
        accent: 'coral',
        count: INSTRUCTION_LINK_MAPPINGS['meeples'].topics.length,
      },
      {
        name: this.ts.instant('instruction.troubleshooting.title'),
        summary: this.ts.instant('instruction.troubleshooting.summary'),
        path: '/instructions/troubleshooting',
        icon: 'table',
        accent: 'teal',
        count: INSTRUCTION_LINK_MAPPINGS['troubleshooting'].topics.length,
      },
    ];
  }

  public get isUserAuthenticated(): boolean {
    return this.authService.getUser !== null;
  }

  public navigateTo(path: string): void {
    this.navigationService.setPreviousUrl(this.router.url);
    this.router.navigateByUrl(path);
  }

  public backNavigateBtn() {
    if (this.isUserAuthenticated) {
      this.tenantRouter.navigateTenant(ROUTE.PROFILE.CORE);
    } else {
      this.router.navigateByUrl(ROUTE.LANDING);
    }
  }
}
