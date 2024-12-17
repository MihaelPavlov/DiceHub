import { MenuTabsService } from './../../../shared/services/menu-tabs.service';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { Router } from '@angular/router';
import { UserAction } from '../../../shared/constants/user-action';
import { PermissionService } from '../../../shared/services/permission.service';
import { NAV_ITEM_LABELS } from '../../../shared/models/nav-items-labels.const';
import { SpaceTableActiveReservations } from '../../../features/reservation-management/components/space-table-reservations/active-list/space-table-active-reservations.component';
import { GameReservations } from '../../../features/reservation-management/components/game-reservations/active-list/game-reservations.component';

@Component({
  selector: 'app-reservation-management-navigation',
  templateUrl: 'reservation-management-navigation.component.html',
  styleUrl: 'reservation-management-navigation.component.scss',
})
export class ReservationManagementNavigationComponent
  implements OnInit, OnDestroy
{
  public activeChildComponent:
    | GameReservations
    | SpaceTableActiveReservations
    | null = null;

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public isAdmin$: Observable<boolean> = this.permissionService.hasUserAction(
    UserAction.GamesCUD
  );

  public header: BehaviorSubject<string> = new BehaviorSubject<string>(
    'Reservations'
  );

  constructor(
    private readonly router: Router,
    private readonly permissionService: PermissionService,
    private readonly menuTabsService: MenuTabsService,
    private readonly cd: ChangeDetectorRef
  ) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
    this.menuTabsService.setActive(NAV_ITEM_LABELS.RESERVATIONS);
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'history-tables', label: 'History Tables' },
      { key: 'history-games', label: 'History Games' },
    ]);
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'history-tables') {
      this.router.navigateByUrl('/reservations/confirmed-tables');
    } else if (key === 'history-games') {
      this.router.navigateByUrl('/games/add-existing-game');
    }
  }

  public onHistoryExpression(): void {
    if (this.activeChildComponent) {
      try {
        this.activeChildComponent.onHistory();
      } catch (error) {}
    }
  }

  public isActiveLink(link: string): boolean {
    return this.router.url.includes(link);
  }

  public onActivate(componentRef: any) {
    this.activeChildComponent = componentRef;
  }

  public onHeader(): void {
    this.activeChildComponent = null;
    this.header.next('Reservations');
    this.router.navigateByUrl('reservations');
  }

  public removeActiveChildComponent(): void {
    this.activeChildComponent = null;
    this.header.next("Reservations")
  }
}
