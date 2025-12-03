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
import { DateHelper } from '../../../../shared/helpers/date-helper';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';

@Component({
    selector: 'app-reservation-confirmation',
    templateUrl: 'reservation-confirmation.dialog.html',
    styleUrl: 'reservation-confirmation.dialog.scss',
    standalone: false
})
export class ReservationConfirmationDialog extends Form {
  override form: Formify<IReservationConfirmationForm>;

  public readonly ReservationStatus = ReservationStatus;
  public readonly DATE_TIME_FORMAT: string = DateHelper.DATE_TIME_FORMAT;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ReservationConfirmation,
    private dialogRef: MatDialogRef<ReservationConfirmationDialog>,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameService: GamesService,
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
  }

  public getUserLanguage(userLanguage: string): string {
    if (
      userLanguage !== SupportLanguages.EN.toString() &&
      userLanguage !== SupportLanguages.BG.toString()
    )
      return userLanguage;

    return this.translateService.instant(`languages_names.${userLanguage}`);
  }

  public get getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public cancelReservation(): void {
    if (this.data.type === ReservationType.Table) {
      this.spaceManagementService
        .cancelReservation(this.data.reservationId)
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                'reservation_management.confirm_dialog.successfully_cancel'
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
      this.gameService.cancelReservation(this.data.reservationId).subscribe({
        next: () => {
          this.toastService.success({
            message: this.translateService.instant(
              'reservation_management.confirm_dialog.successfully_cancel'
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
              message: this.translateService.instant(
                'reservation_management.confirm_dialog.successfully_declined'
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
        .declinedReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                'reservation_management.confirm_dialog.successfully_declined'
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
              message: this.translateService.instant(
                'reservation_management.confirm_dialog.successfully_approved'
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
        .approveReservation(
          this.data.reservationId,
          this.form.controls.publicNote.value,
          this.form.controls.internalNote.value
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                'reservation_management.confirm_dialog.successfully_approved'
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
