import { Component, Injector, OnDestroy } from '@angular/core';
import { ReservationManagementNavigationComponent } from '../../../../pages/reservation-management/page/reservation-management-navigation.component';
import { Observable } from 'rxjs';
import { IReservedTable } from '../../../../entities/space-management/models/reserved-table.model';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';

@Component({
  selector: 'app-space-table-reservations',
  templateUrl: 'space-table-reservations.component.html',
  styleUrl: 'space-table-reservations.component.scss',
})
export class SpaceTableReservations implements OnDestroy {
  private reservationNavigationRef!: ReservationManagementNavigationComponent | null;

  public reservedGames$!: Observable<IReservedTable[]>;
  public showFilter: boolean = false;
  public expandedItemId: number | null = null;
  public leftArrowKey: string = 'arrow_circle_left';
  public rightArrowKey: string = 'arrow_circle_right';

  constructor(
    private readonly injector: Injector,
    private readonly spaceManagementService: SpaceManagementService
  ) {
    this.reservationNavigationRef = this.injector.get(
      ReservationManagementNavigationComponent,
      null
    );
  }
  public toggleItem(reservationId: number): void {
    this.expandedItemId =
      this.expandedItemId === reservationId ? null : reservationId;
  }

  public isExpanded(reservationId: number): boolean {
    return this.expandedItemId === reservationId;
  }

  public ngOnInit(): void {
    this.reservedGames$ = this.spaceManagementService.getReservedTableList();
  }

  public ngOnDestroy(): void {
    if (this.reservationNavigationRef)
      this.reservationNavigationRef.removeActiveChildComponent();
  }
}
