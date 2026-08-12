import { SpaceManagementService } from './../../../entities/space-management/api/space-management.service';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  HostListener,
} from '@angular/core';
import { IMenuItemInterface } from '../models/menu-item.interface';
import { NavigationEnd, Router } from '@angular/router';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  filter,
  of,
  Subject,
  takeUntil,
} from 'rxjs';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../shared/services/menu-tabs.service';
import { AuthService } from '../../../entities/auth/auth.service';
import { UserRole } from '../../../entities/auth/enums/roles.enum';
import { GamesService } from '../../../entities/games/api/games.service';
import { ROUTE } from '../../../shared/configs/route.config';
import { TenantRouter } from '../../../shared/helpers/tenant-router';
import { TenantContextService } from '../../../shared/services/tenant-context.service';

@Component({
  selector: 'app-navigation-menu',
  templateUrl: 'navigation-menu.component.html',
  styleUrl: 'navigation-menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: false,
})
export class NavigationMenuComponent implements OnInit {
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
    private readonly tenantRouter: TenantRouter,
    private readonly menuTabsService: MenuTabsService,
    private readonly authService: AuthService,
    private readonly cd: ChangeDetectorRef,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameService: GamesService,
    private readonly tenantContextService: TenantContextService
  ) {}

  public isSuperAdmin(): boolean {
    return this.authService.getUser?.role === UserRole.SuperAdmin && this.authService.getUser?.tenantId === 'system';
  }

  public isTenantVisit(): boolean {
    return this.isSuperAdmin() && this.tenantContextService.hasTenant() && !this.router.url.startsWith('/admin');
  }

  public ngOnInit(): void {
    this.updateMenuItems();
    if (this.authService.getUser?.role !== UserRole.User) {
      this.refreshForAnyActiveReservations();
    }

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
        this.activeLink = (navEvent as NavigationEnd).url.split('/')[2];
        this.updateMenuItemsWithPage(this.activeLink);
        this.cd.detectChanges();
      });
    if (this.authService.getUser?.role !== UserRole.User) {
      this.subscriptionRefreshForAnyActiveReservations = setInterval(
        () => this.refreshForAnyActiveReservations(),
        10000
      );
    }
  }

  public toggleInteractive(event: MouseEvent): void {
    event.stopPropagation();
    (event.currentTarget as HTMLElement).classList.toggle('active');
  }

  @HostListener('document:click', ['$event'])
  public closeInteractive(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.interactive-option')) {
      document.querySelector('.interactive-option.active')?.classList.remove('active');
    }
  }

  public refreshForAnyActiveReservations(): void {
    combineLatest([
      this.gameService.getReservations_BackgroundRequest().pipe(
        catchError((err) => {
          console.warn('Game reservations failed silently', err);
          return of([]);
        })
      ),
      this.spaceManagementService
        .getActiveReservedTableList_BackgroundRequest()
        .pipe(
          catchError((err) => {
            console.warn('Table reservations failed silently', err);
            return of([]);
          })
        ),
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
    if (this.isSuperAdmin()) return;
    const page = location.pathname.split('/')[2];
    const isReservationsPage = page === 'reservations';

    this.leftMenuItems = this.leftMenuItems.filter(
      (item) => item.route !== this.tenantRouter.buildTenantUrl('/reservations')
    );

    this.leftMenuItems.push({
      label: NAV_ITEM_LABELS.RESERVATIONS.toLowerCase(),
      class: isReservationsPage ? 'active' : '',
      forceActive: isReservationsPage,
      isAlertActive: hasActive,
      enabled: true,
      visible: true,
      icon: '/shared/assets/images/icons/menu_book-icon.svg',
      icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg',
      route: this.tenantRouter.buildTenantUrl('/reservations'),
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
    this.tenantRouter.navigateTenant('space/home');
  }

  public isDesktopActionActive(page: string): boolean {
    return this.activeLink === page;
  }

  public navigateToChallenges(): void {
    this.tenantRouter.navigateTenant('challenges/home');
  }

  public exitTenantPreview(): void {
    this.tenantContextService.clearTenant();
    this.router.navigate(['/admin/tenants']);
  }

  public logoutSuperAdmin(): void {
    const finishLogout = () => {
      this.tenantContextService.clearTenant();
      this.router.navigate(['/admin/login']);
    };

    this.authService.logout(true).subscribe({
      next: finishLogout,
      error: finishLogout,
    });
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
    if (this.isSuperAdmin() && !this.isTenantVisit()) {
      this.leftMenuItems = [{ label: 'tenants', class: page === 'tenants' ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/tenants', sectionBreak: true }];
      this.rightMenuItems = [{ label: 'applicants', class: page === 'applicants' ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/applicants' }];
      return;
    }
    if (this.isTenantVisit()) {
      const tenantRoute = (path: string) => this.tenantRouter.buildTenantUrl(path);
      const normalizeUrl = (url: string) => url.replace(/^\/+/, '').replace(/\/{2,}/g, '/');
      const isTenantRouteActive = (path: string) => normalizeUrl(this.router.url) === normalizeUrl(tenantRoute(path));
      this.leftMenuItems = [
        { label: 'games', class: isTenantRouteActive('/games/library') ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/kid_star-icon.svg', icon_color: '/shared/assets/images/icons/kid_star-icon-blue.svg', route: tenantRoute('/games/library') },
        { label: 'meeple', class: isTenantRouteActive('/meeples/find') ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/group-icon.svg', icon_color: '/shared/assets/images/icons/group-icon-blue.svg', route: tenantRoute('/meeples/find') },
        { label: 'reservations', class: isTenantRouteActive('/reservations') ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/menu_book-icon.svg', icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg', route: tenantRoute('/reservations') },
      ];
      this.leftMenuItems.push(
        { label: 'events', class: isTenantRouteActive('/events/home') ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/stadium-icon.svg', icon_color: '/shared/assets/images/icons/stadium-icon-blue.svg', route: tenantRoute('/events/home') },
        { label: 'profile', class: isTenantRouteActive('/profile') ? 'active' : '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: tenantRoute('/profile') },
      );
      this.rightMenuItems = [
        { label: 'tenants', class: '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/tenants', sectionBreak: true },
        { label: 'applicants', class: '', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/applicants' },
      ];
      return;
    }
    this.leftMenuItems = [
      {
        label: NAV_ITEM_LABELS.GAMES.toLowerCase(),
        class:
          page === this.tenantRouter.buildTenantUrl('/games/library')
            ? 'active'
            : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/kid_star-icon.svg',
        icon_color: '/shared/assets/images/icons/kid_star-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/games/library'),
      },
    ];

    if (this.authService.getUser?.role !== UserRole.User) {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.RESERVATIONS.toLowerCase(),
        class:
          page === this.tenantRouter.buildTenantUrl('/reservations')
            ? 'active'
            : '',
        isAlertActive: true,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/menu_book-icon.svg',
        icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/reservations'),
      });
    } else {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.MEEPLE.toLowerCase(),
        class:
          page === this.tenantRouter.buildTenantUrl('/meeples/find')
            ? 'active'
            : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/group-icon.svg',
        icon_color: '/shared/assets/images/icons/group-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/meeples/find'),
      });
    }

    this.rightMenuItems = [
      {
        label: NAV_ITEM_LABELS.EVENTS.toLowerCase(),
        class:
          page === this.tenantRouter.buildTenantUrl('/events/home')
            ? 'active'
            : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/stadium-icon.svg',
        icon_color: '/shared/assets/images/icons/stadium-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/events/home'),
      },
      {
        label: NAV_ITEM_LABELS.PROFILE.toLowerCase(),
        class:
          page === this.tenantRouter.buildTenantUrl('/profile') ? 'active' : '',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/account_circle-icon.svg',
        icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/profile'),
      },
    ];
  }

  public navigateToQrCodeScanner(): void {
    this.tenantRouter.navigateTenant(ROUTE.QR_CODE_SCANNER);
  }

  private updateMenuItems() {
    let page: string = location.pathname.split('/')[2];
    this.activeLink = page;

    this.updateMenuItemsWithPage(page);
  }
}
