import { Component, Inject } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { IReservationConfirmationForm } from '../models/reservation-confirmation-form.model';
import { IGetReservationById } from '../../../../entities/space-management/models/get-reservation-by-id.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { ReservationDetailsActions } from '../enums/reservation-details-actions.enum';

@Component({
  selector: 'app-reservation-details',
  templateUrl: 'reservation-details.dialog.html',
  styleUrl: 'reservation-details.dialog.scss',
})
export class ReservationDetailsDialog extends Form {
  override form: Formify<IReservationConfirmationForm>;
  public ReservationDetailsActions = ReservationDetailsActions;
  public reservation!: IGetReservationById;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: { reservationId: number; action: ReservationDetailsActions },
    private readonly spaceManagementService: SpaceManagementService,
    private dialogRef: MatDialogRef<ReservationDetailsDialog>,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);
    this.form = this.initFormGroup();

    this.spaceManagementService
      .getReservationById(this.data.reservationId)
      .subscribe({
        next: (reservation) => {
          console.log(reservation);
          this.reservation = reservation;
          this.form.patchValue({
            internalNote: reservation.internalNote,
            publicNote: reservation.publicNote,
          });
        },
      });
  }

  public updateReservation(): void {
    this.spaceManagementService
      .updateReservation(
        this.data.reservationId,
        this.form.controls.publicNote.value,
        this.form.controls.internalNote.value
      )
      .subscribe({
        next: () => {
          this.toastService.success({
            message: AppToastMessage.ChangesSaved,
            type: ToastType.Success,
          });

          this.dialogRef.close(true);
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
  }

  public deleteReservation(): void {
    this.spaceManagementService
      .deleteReservation(this.data.reservationId)
      .subscribe({
        next: () => {
          this.toastService.success({
            message: AppToastMessage.ChangesApplied,
            type: ToastType.Success,
          });

          this.dialogRef.close(true);
        },
        error: () => {
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
      });
  }

  protected override getControlDisplayName(controlName: string): string {
    return '';
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      internalNote: new FormControl<string>('', []),
      publicNote: new FormControl<string>('', []),
    });
  }
}
