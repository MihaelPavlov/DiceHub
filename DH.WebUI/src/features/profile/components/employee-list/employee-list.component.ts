import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { IUser } from '../../../../entities/profile/models/user.model';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { FULL_ROUTE, ROUTE } from '../../../../shared/configs/route.config';
import { NavigationService } from '../../../../shared/services/navigation-service';
import { BehaviorSubject } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { EmployeeConfirmDeleteDialog } from '../../dialogs/employee-confirm-delete/employee-confirm-delete.component';
import { ToastService } from '../../../../shared/services/toast.service';
import { TranslateService } from '@ngx-translate/core';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

@Component({
  selector: 'app-profile',
  templateUrl: 'employee-list.component.html',
  styleUrl: 'employee-list.component.scss',
})
export class EmployeeListComponent implements OnDestroy {
  public employees: IUser[] = [];
  public readonly ImageEntityType = ImageEntityType;
  public expandedEmployeeId: BehaviorSubject<string | null> =
    new BehaviorSubject<string | null>(null);

  constructor(
    private readonly menuTabsService: MenuTabsService,
    private readonly usersService: UsersService,
    private readonly router: Router,
    private readonly navigationService: NavigationService,
    private readonly dialog: MatDialog,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {
    this.fetchEmployeeList();
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }

  public fetchEmployeeList(): void {
    this.usersService.getEmployeeList().subscribe({
      next: (employees) => (this.employees = employees ?? []),
      error: () => {
        this.toastService.success({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Success,
        });
      },
    });
  }
  public toggleItem(reservationId: string): void {
    this.expandedEmployeeId.next(
      this.expandedEmployeeId.value === reservationId ? null : reservationId
    );
  }

  public isExpanded(reservationId: string): boolean {
    return this.expandedEmployeeId.value === reservationId;
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public onAdd(): void {
    this.navigationService.setPreviousUrl(FULL_ROUTE.PROFILE.EMPLOYEES);
    this.router.navigateByUrl(FULL_ROUTE.PROFILE.ADD_EMPLOYEE);
  }

  public onBack(): void {
    this.router.navigateByUrl(ROUTE.PROFILE.CORE);
  }

  public onEdit(employeeId: string, event?: TouchEvent | MouseEvent): void {
    if (event) event.stopPropagation();
    this.navigationService.setPreviousUrl(FULL_ROUTE.PROFILE.EMPLOYEES);
    this.router.navigateByUrl(FULL_ROUTE.PROFILE.UPDATE_BY_ID(employeeId));
  }

  public openDeleteDialog(
    employeeId: string,
    event?: TouchEvent | MouseEvent
  ): void {
    if (event) event.stopPropagation();

    const dialogRef = this.dialog.open(EmployeeConfirmDeleteDialog, {
      data: { employeeId },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchEmployeeList();
      }
    });
  }
}
