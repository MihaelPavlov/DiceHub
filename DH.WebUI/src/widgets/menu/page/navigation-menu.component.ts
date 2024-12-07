import { SpaceManagementService } from './../../../entities/space-management/api/space-management.service';
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
} from '@angular/core';
import { IMenuItemInterface } from '../models/menu-item.interface';
import { NavigationEnd, Router } from '@angular/router';
import {
  BehaviorSubject,
  combineLatest,
  filter,
  Subject,
  takeUntil,
} from 'rxjs';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { AuthService } from '../../../entities/auth/auth.service';
import { UserRole } from '../../../entities/auth/enums/roles.enum';
import { GamesService } from '../../../entities/games/api/games.service';

@Component({
  selector: 'app-navigation-menu',
  templateUrl: 'navigation-menu.component.html',
  styleUrl: 'navigation-menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavigationMenuComponent implements OnInit, AfterViewInit {
  public areAnyActiveReservation!: BehaviorSubject<boolean>;
  public leftMenuItems: IMenuItemInterface[] = [];
  public rightMenuItems: IMenuItemInterface[] = [];
  public menuItemWithForceActiveExists: boolean = false;
  private destroy$: Subject<boolean> = new Subject<boolean>();
  public activeLink = NAV_ITEM_LABELS.GAMES;
  public subscriptionRefreshForAnyActiveReservations!: any;

  constructor(
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly cd: ChangeDetectorRef,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameService: GamesService
  ) {
    this.updateMenuItems();
    if (this.authService.getUser?.role !== UserRole.User) {
      this.refreshForAnyActiveReservations();
    }
  }

  public ngOnInit(): void {
    this.menuTabsService.activeTab$
      .pipe(takeUntil(this.destroy$))
      .subscribe((label) => {
        this.setActiveTab(label);
        this.cd.detectChanges();
      });
    this.router.events
      .pipe(
        takeUntil(this.destroy$),
        filter((event) => event instanceof NavigationEnd)
      )
      .subscribe((navEvent: any) => {
        this.activeLink = (navEvent as NavigationEnd).url.split('/')[1];
      });
    if (this.authService.getUser?.role !== UserRole.User) {
      this.subscriptionRefreshForAnyActiveReservations = setInterval(
        () => this.refreshForAnyActiveReservations(),
        10000
      );
    }
  }

  public refreshForAnyActiveReservations(): void {
    combineLatest([
      this.gameService.getReservations(),
      this.spaceManagementService.getActiveReservedTableList(),
    ]).subscribe({
      next: ([gameReservations, tableReservations]) => {
        const gameActiveReservations =
          this.filterActiveReservations(gameReservations);
        const tableActiveReservations =
          this.filterActiveReservations(tableReservations);

        const anyActiveReservations =
          gameActiveReservations.length > 0 ||
          tableActiveReservations.length > 0;

        this.updateLeftMenuItems(anyActiveReservations);

        this.cd.detectChanges();
      },
    });
  }
  private updateLeftMenuItems(hasActive: boolean): void {
    const page = location.pathname.split('/')[1];
    const isReservationsPage = page === 'reservations';

    this.leftMenuItems = this.leftMenuItems.filter(
      (item) => item.route !== '/reservations'
    );

    this.leftMenuItems.push({
      label: NAV_ITEM_LABELS.BOOKING,
      class: isReservationsPage ? 'active' : '',
      forceActive: isReservationsPage,
      isAlertActive: hasActive,
      enabled: true,
      visible: true,
      icon: 'menu_book',
      route: '/reservations',
    });
  }
  private filterActiveReservations(reservations: any[]): any[] {
    return reservations?.filter((x) => x.isActive) || [];
  }

  public ngAfterViewInit(): void {
    this.onInitJS();
  }

  public ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
    if (this.subscriptionRefreshForAnyActiveReservations)
      clearInterval(this.subscriptionRefreshForAnyActiveReservations);
  }

  public navigateToSpaceManagement(): void {
    this.router.navigate(['/space/home']);
  }

  public navigateToChallenges(): void {
    this.router.navigate(['/challenges/home']);
  }

  public setActiveTab(label: string) {
    let menuItem = this.leftMenuItems
      .concat(this.rightMenuItems)
      .find(
        (item) =>
          item.label.toString().toLowerCase() === label.toString().toLowerCase()
      );
    if (menuItem) {
      menuItem.forceActive = true;
      this.menuItemWithForceActiveExists = true;
    } else {
      this.leftMenuItems.forEach((item) => (item.forceActive = false));
      this.rightMenuItems.forEach((item) => (item.forceActive = false));

      this.menuItemWithForceActiveExists = false;
    }
  }

  public updateMenuItemsWithPage(page: string) {
    this.leftMenuItems = [
      {
        label: NAV_ITEM_LABELS.GAMES,
        class: page === '/games/library' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: 'kid_star',
        route: '/games/library',
      },
    ];

    if (this.authService.getUser?.role !== UserRole.User) {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.BOOKING,
        class: page === '/reservations' ? 'active' : '',
        isAlertActive: true,
        enabled: true,
        visible: true,
        icon: 'menu_book',
        route: '/reservations',
      });
    } else {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.MEEPLE,
        class: page === '/meeples/find' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: 'group',
        route: '/meeples/find',
      });
    }

    this.rightMenuItems = [
      {
        label: NAV_ITEM_LABELS.EVENTS,
        class: page === '/events/home' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: 'stadium',
        route: '/events/home',
      },
      {
        label: NAV_ITEM_LABELS.PROFILE,
        class: page === '/profile' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: 'account_circle',
        route: '/profile',
      },
    ];
  }

  public navigateToQrCodeScanner(): void {
    this.router.navigateByUrl('qr-code-scanner');
  }

  private updateMenuItems() {
    let page: string = location.pathname.split('/')[1];
    this.updateMenuItemsWithPage(page);
  }

  private onInitJS(): void {
    const interactiveOption = document.querySelector(
      '.interactive-option'
    ) as HTMLElement;

    interactiveOption.addEventListener('click', function (event) {
      event.stopPropagation(); // Prevents the click event from propagating to the document body

      interactiveOption.classList.toggle('active');
    });

    // Hide the interactive option when clicking anywhere outside of it
    document.documentElement.addEventListener('mousedown', function (event) {
      if (!interactiveOption.contains(event.target as Node)) {
        interactiveOption.classList.remove('active');
      }
    });
  }
}
