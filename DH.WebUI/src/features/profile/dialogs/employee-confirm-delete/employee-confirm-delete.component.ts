import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { UsersService } from '../../../../entities/profile/api/user.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-employee-confirm-delete-dialog',
    templateUrl: 'employee-confirm-delete.component.html',
    styleUrl: 'employee-confirm-delete.component.scss',
    standalone: false
})
export class EmployeeConfirmDeleteDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<EmployeeConfirmDeleteDialog>,
    private readonly userService: UsersService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService
  ) {}

  public delete(): void {
    this.userService.deleteEmployee(this.data.employeeId).subscribe({
      next: () => {
        this.toastService.success({
          message: this.translateService.instant(
            'profile.employee_list.deletion_successfully'
          ),
          type: ToastType.Success,
        });
        this.dialogRef.close(true);
      },
      error: (error) => {
        this.toastService.error({
          message: this.translateService.instant(
            'profile.employee_list.deletion_unsuccessfully'
          ),
          type: ToastType.Error,
        });

        this.dialogRef.close(false);
      },
    });
  }
}
