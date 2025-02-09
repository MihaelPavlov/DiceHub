import { SpaceManagementService } from './../../../entities/space-management/api/space-management.service';
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
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
import { ROUTE } from '../../../shared/configs/route.config';

@Component({
  selector: 'app-navigation-menu',
  templateUrl: 'navigation-menu.component.html',
  styleUrl: 'navigation-menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavigationMenuComponent implements OnInit, AfterViewInit {
  @ViewChild('interactiveOption') interactiveOption!: ElementRef<HTMLElement>;

  public areAnyActiveReservation!: BehaviorSubject<boolean>;
  public leftMenuItems: IMenuItemInterface[] = [];
  public rightMenuItems: IMenuItemInterface[] = [];
  public menuItemWithForceActiveExists: boolean = false;
  private destroy$: Subject<boolean> = new Subject<boolean>();
  public activeLink = NAV_ITEM_LABELS.GAMES;
  public subscriptionRefreshForAnyActiveReservations!: any;
  public eventLis: any;
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

  public ngAfterViewInit(): void {
    this.interactiveOption.nativeElement.addEventListener('click', (event) => {
      event.stopPropagation();
      this.interactiveOption.nativeElement.classList.toggle('active');
    });

    document.documentElement.addEventListener('click', (event) => {
      if (
        !this.interactiveOption.nativeElement.contains(event.target as Node)
      ) {
        this.interactiveOption.nativeElement.classList.remove('active');
      }
    });
  }

  public refreshForAnyActiveReservations(): void {
    combineLatest([
      this.gameService.getReservations_BackgroundRequest(),
      this.spaceManagementService.getActiveReservedTableList_BackgroundRequest(),
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
      label: NAV_ITEM_LABELS.RESERVATIONS,
      class: isReservationsPage ? 'active' : '',
      forceActive: isReservationsPage,
      isAlertActive: hasActive,
      enabled: true,
      visible: true,
      icon: '/shared/assets/images/icons/menu_book-icon.svg',
      icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg',
      route: '/reservations',
    });
  }
  private filterActiveReservations(reservations: any[]): any[] {
    return reservations?.filter((x) => x.isActive) || [];
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
        icon: '/shared/assets/images/icons/kid_star-icon.svg',
        icon_color: '/shared/assets/images/icons/kid_star-icon-blue.svg',
        route: '/games/library',
      },
    ];

    if (this.authService.getUser?.role !== UserRole.User) {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.RESERVATIONS,
        class: page === '/reservations' ? 'active' : '',
        isAlertActive: true,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/menu_book-icon.svg',
        icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg',
        route: '/reservations',
      });
    } else {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.MEEPLE,
        class: page === '/meeples/find' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/group-icon.svg',
        icon_color: '/shared/assets/images/icons/group-icon-blue.svg',
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
        icon: '/shared/assets/images/icons/stadium-icon.svg',
        icon_color: '/shared/assets/images/icons/stadium-icon-blue.svg',
        route: '/events/home',
      },
      {
        label: NAV_ITEM_LABELS.PROFILE,
        class: page === '/profile' ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/account_circle-icon.svg',
        icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg',
        route: '/profile',
      },
    ];
  }

  public navigateToQrCodeScanner(): void {
    this.router.navigateByUrl(ROUTE.QR_CODE_SCANNER);
  }

  private updateMenuItems() {
    let page: string = location.pathname.split('/')[1];
    this.updateMenuItemsWithPage(page);
  }
}
