import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IMenuItem } from '../../../shared/models/menu-item.model';
import { Router } from '@angular/router';
import { UserAction } from '../../../shared/constants/user-action';
import { PermissionService } from '../../../shared/services/permission.service';
import { GameReservations } from '../../../features/reservation-management/components/game-reservations/game-reservations.component';
import { SpaceTableReservations } from '../../../features/reservation-management/components/space-table-reservations/space-table-reservations.component';

@Component({
  selector: 'app-reservation-management-navigation',
  templateUrl: 'reservation-management-navigation.component.html',
  styleUrl: 'reservation-management-navigation.component.scss',
})
export class ReservationManagementNavigationComponent implements OnInit {
  public activeChildComponent:
    | GameReservations
    | SpaceTableReservations
    | null = null;

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  public isAdmin$: Observable<boolean> = this.permissionService.hasUserAction(
    UserAction.GamesCUD
  );
  constructor(
    private readonly router: Router,
    private readonly permissionService: PermissionService,
    private readonly cd: ChangeDetectorRef
  ) {
    this.handleMenuItemClick = this.handleMenuItemClick.bind(this);
  }

  public ngOnInit(): void {
    this.menuItems.next([
      { key: 'add-game', label: 'Add Game' },
      { key: 'add-existing-game', label: 'Add Existing Game' },
      { key: 'reserved-games', label: 'Reserved Games' },
    ]);
  }

  public handleMenuItemClick(key: string): void {
    if (key === 'add-game') {
      this.router.navigateByUrl('/games/add');
    } else if (key === 'add-existing-game') {
      this.router.navigateByUrl('/games/add-existing-game');
    } else if (key === 'reserved-games') {
      this.router.navigateByUrl('/games/reservations');
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
    this.router.navigateByUrl('reservations');
  }

  public removeActiveChildComponent(): void {
    this.activeChildComponent = null;
  }
}
