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
  defaultIfEmpty,
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
  private currentForcedLabel: string = '';
  private destroy$: Subject<boolean> = new Subject<boolean>();
  public activeLink = NAV_ITEM_LABELS.GAMES;
  // Last known "there is at least one active reservation" result. Drives the red
  // dot on the Reservations tab. Starts false and is only ever set from the
  // getReservations/getActiveReservedTableList responses in
  // refreshForAnyActiveReservations() - never hardcoded, or the dot shows with
  // no reservations until the first background poll corrects it.
  public anyActiveReservations = false;
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
        // updateMenuItemsWithPage() rebuilds leftMenuItems/rightMenuItems as
        // new objects, which would silently drop whichever item setActiveTab()
        // had marked forceActive on the previous array. Reapply it here so the
        // forced tab (e.g. a nested page that called menuTabsService.setActive)
        // still shows as active after the rebuild.
        this.applyForcedActiveLabel();
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
        }),
        // The auth interceptor turns a background 401 into EMPTY - the stream
        // completes without ever emitting, which makes combineLatest hang and
        // the red dot never update. Guarantee a value.
        defaultIfEmpty<any[], any[]>([])
      ),
      this.spaceManagementService
        .getActiveReservedTableList_BackgroundRequest()
        .pipe(
          catchError((err) => {
            console.warn('Table reservations failed silently', err);
            return of([]);
          }),
          defaultIfEmpty<any[], any[]>([])
        ),
    ]).subscribe({
      next: ([gameReservations, tableReservations]) => {
        const gameActiveReservations =
          this.filterActiveReservations(gameReservations);
        const tableActiveReservations =
          this.filterActiveReservations(tableReservations);

        this.anyActiveReservations =
          gameActiveReservations.length > 0 ||
          tableActiveReservations.length > 0;

        this.updateLeftMenuItems(this.anyActiveReservations);

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
    this.currentForcedLabel = label;
    this.applyForcedActiveLabel();
  }

  private applyForcedActiveLabel(): void {
    // Reset every item first - previously this only ever set the new match's
    // forceActive to true without clearing the rest, so a tab force-activated
    // once could never be un-stuck by a later navigation.
    this.leftMenuItems.forEach((item) => (item.forceActive = false));
    this.rightMenuItems.forEach((item) => (item.forceActive = false));

    const menuItem = this.leftMenuItems
      .concat(this.rightMenuItems)
      .find(
        (item) =>
          item.label.toString().toLowerCase() === this.currentForcedLabel.toString().toLowerCase()
      );

    if (menuItem) {
      menuItem.forceActive = true;
      this.menuItemWithForceActiveExists = true;
    } else {
      this.menuItemWithForceActiveExists = false;
    }
  }

  public updateMenuItemsWithPage(page: string) {
    if (this.isSuperAdmin() && !this.isTenantVisit()) {
      this.leftMenuItems = [{ label: 'tenants', forceActive: page === 'tenants', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/tenants', sectionBreak: true }];
      this.rightMenuItems = [{ label: 'applicants', forceActive: page === 'applicants', isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/applicants' }];
      this.menuItemWithForceActiveExists = this.leftMenuItems.concat(this.rightMenuItems).some((item) => item.forceActive);
      return;
    }
    if (this.isTenantVisit()) {
      const tenantRoute = (path: string) => this.tenantRouter.buildTenantUrl(path);
      const normalizeUrl = (url: string) => url.replace(/^\/+/, '').replace(/\/{2,}/g, '/');
      const isTenantRouteActive = (path: string) => normalizeUrl(this.router.url) === normalizeUrl(tenantRoute(path));
      this.leftMenuItems = [
        { label: 'games', forceActive: isTenantRouteActive('/games/library'), isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/kid_star-icon.svg', icon_color: '/shared/assets/images/icons/kid_star-icon-blue.svg', route: tenantRoute('/games/library') },
        { label: 'meeple', forceActive: isTenantRouteActive('/meeples/find'), isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/group-icon.svg', icon_color: '/shared/assets/images/icons/group-icon-blue.svg', route: tenantRoute('/meeples/find') },
        { label: 'reservations', forceActive: isTenantRouteActive('/reservations'), isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/menu_book-icon.svg', icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg', route: tenantRoute('/reservations') },
      ];
      this.leftMenuItems.push(
        { label: 'events', forceActive: isTenantRouteActive('/events/home'), isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/stadium-icon.svg', icon_color: '/shared/assets/images/icons/stadium-icon-blue.svg', route: tenantRoute('/events/home') },
        { label: 'profile', forceActive: isTenantRouteActive('/profile'), isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: tenantRoute('/profile') },
      );
      this.rightMenuItems = [
        { label: 'tenants', forceActive: false, isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/tenants', sectionBreak: true },
        { label: 'applicants', forceActive: false, isAlertActive: false, enabled: true, visible: true, icon: '/shared/assets/images/icons/account_circle-icon.svg', icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg', route: '/admin/applicants' },
      ];
      this.menuItemWithForceActiveExists = this.leftMenuItems.concat(this.rightMenuItems).some((item) => item.forceActive);
      return;
    }
    // `page` is the bare route segment right after the tenant prefix (e.g.
    // 'games', 'reservations') - compare against that directly, not against
    // tenantRouter.buildTenantUrl(...)'s full tenant-prefixed path, which
    // never equals a single segment and left every tab's active state broken.
    this.leftMenuItems = [
      {
        label: NAV_ITEM_LABELS.GAMES.toLowerCase(),
        forceActive: page === 'games',
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
        forceActive: page === 'reservations',
        isAlertActive: this.anyActiveReservations,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/menu_book-icon.svg',
        icon_color: '/shared/assets/images/icons/menu_book-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/reservations'),
      });
    } else {
      this.leftMenuItems.push({
        label: NAV_ITEM_LABELS.MEEPLE.toLowerCase(),
        forceActive: page === 'meeples',
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
        forceActive: page === 'events',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/stadium-icon.svg',
        icon_color: '/shared/assets/images/icons/stadium-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/events/home'),
      },
      {
        label: NAV_ITEM_LABELS.PROFILE.toLowerCase(),
        forceActive: page === 'profile',
        isAlertActive: false,
        enabled: true,
        visible: true,
        icon: '/shared/assets/images/icons/account_circle-icon.svg',
        icon_color: '/shared/assets/images/icons/account_circle-icon-blue.svg',
        route: this.tenantRouter.buildTenantUrl('/profile'),
      },
    ];

    this.menuItemWithForceActiveExists = this.leftMenuItems
      .concat(this.rightMenuItems)
      .some((item) => item.forceActive);
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
