import { ReservationStatus } from './../../../../shared/enums/reservation-status.enum';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastService } from '../../../../shared/services/toast.service';
import { SpaceManagementService } from '../../../../entities/space-management/api/space-management.service';
import { ReservationType } from '../../enums/reservation-type.enum';
import { ToastType } from '../../../../shared/models/toast.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ReservationConfirmation } from '../models/reservation-confirmation-dialog.model';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { IReservationConfirmationForm } from '../models/reservation-confirmation-form.model';
import { GamesService } from '../../../../entities/games/api/games.service';

@Component({
  selector: 'app-reservation-confirmation',
  templateUrl: 'reservation-confirmation.dialog.html',
  styleUrl: 'reservation-confirmation.dialog.scss',
})
export class ReservationConfirmationDialog extends Form {
  override form: Formify<IReservationConfirmationForm>;

  public ReservationStatus = ReservationStatus;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ReservationConfirmation,
    private dialogRef: MatDialogRef<ReservationConfirmationDialog>,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameService: GamesService,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);
    this.form = this.initFormGroup();
  }

  public declineReservation(): void {
    if (this.data.type === ReservationType.Table) {
      this.spaceManagementService
        .declinedReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is declined',
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
    } else if (this.data.type === ReservationType.Game) {
      this.gameService
        .declinedReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is declined',
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
  }

  public approveReservation(): void {
    if (this.data.type === ReservationType.Table) {
      this.spaceManagementService
        .approveReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is approved',
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
    } else if (this.data.type === ReservationType.Game) {
      this.gameService
        .approveReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: 'Reservation is approved',
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
