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
import { ReservationType } from '../../enums/reservation-type.enum';
import { GamesService } from '../../../../entities/games/api/games.service';
import { DateHelper } from '../../../../shared/helpers/date-helper';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';

@Component({
    selector: 'app-reservation-details',
    templateUrl: 'reservation-details.dialog.html',
    styleUrl: 'reservation-details.dialog.scss',
    standalone: false
})
export class ReservationDetailsDialog extends Form {
  override form: Formify<IReservationConfirmationForm>;

  public reservation!: IGetReservationById;

  public readonly ReservationDetailsActions = ReservationDetailsActions;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      reservationId: number;
      action: ReservationDetailsActions;
      type: ReservationType;
    },
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameService: GamesService,
    private readonly dialogRef: MatDialogRef<ReservationDetailsDialog>,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();

    if (data.type === ReservationType.Table) {
      this.spaceManagementService
        .getReservationById(this.data.reservationId)
        .subscribe({
          next: (reservation) => {
            this.reservation = reservation;
            this.form.patchValue({
              internalNote: reservation.internalNote,
              publicNote: reservation.publicNote,
            });
          },
        });
    } else if (data.type === ReservationType.Game) {
      this.gameService.getReservationById(this.data.reservationId).subscribe({
        next: (reservation) => {
          this.reservation = reservation;
          this.form.patchValue({
            internalNote: reservation.internalNote,
            publicNote: reservation.publicNote,
          });
        },
      });
    }
  }

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public updateReservation(): void {
    if (this.data.type === ReservationType.Table) {
      this.spaceManagementService
        .updateReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });

            this.dialogRef.close(true);
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });
          },
        });
    } else if (this.data.type === ReservationType.Game) {
      this.gameService
        .updateReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });

            this.dialogRef.close(true);
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public deleteReservation(): void {
    if (this.data.type === ReservationType.Table) {
      this.spaceManagementService
        .deleteReservation(this.data.reservationId)
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesApplied
              ),
              type: ToastType.Success,
            });

            this.dialogRef.close(true);
          },
          error: () => {
            this.toastService.error({
              message: this.translateService.instant(
                AppToastMessage.SomethingWrong
              ),
              type: ToastType.Error,
            });
          },
        });
    } else if (this.data.type === ReservationType.Game) {
      this.gameService.deleteReservation(this.data.reservationId).subscribe({
        next: () => {
          this.toastService.success({
            message: this.translateService.instant(
              AppToastMessage.ChangesApplied
            ),
            type: ToastType.Success,
          });

          this.dialogRef.close(true);
        },
        error: () => {
          this.toastService.error({
            message: this.translateService.instant(
              AppToastMessage.SomethingWrong
            ),
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
