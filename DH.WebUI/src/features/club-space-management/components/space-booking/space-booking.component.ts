import {
  animate,
  keyframes,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { Component, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { Form } from '../../../../shared/components/form/form.component';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { Formify } from '../../../../shared/models/form.model';
import { ToastType } from '../../../../shared/models/toast.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { TenantSettingsService } from './../../../../entities/common/api/tenant-settings.service';
import { SpaceManagementService } from './../../../../entities/space-management/api/space-management.service';
import { DiceRollerComponent } from './components/dice-scroller/dice-roller.component';
import { BehaviorSubject } from 'rxjs';
import { IMenuItem } from '../../../../shared/models/menu-item.model';
import { ControlsMenuComponent } from '../../../../shared/components/menu/controls-menu.component';
import { TranslateService } from '@ngx-translate/core';

interface ICreateSpaceReservation {
  reservationDate: Date;
}
@Component({
  selector: 'app-club-space-booking',
  templateUrl: 'space-booking.component.html',
  styleUrl: 'space-booking.component.scss',
  animations: [
    trigger('flowLeft', [
      state('start', style({ transform: 'translateX(-50px)', opacity: 1 })),
      state('reverse', style({ transform: 'translateX(0)', opacity: 1 })),
      transition('* => start', [
        style({ transform: 'translateX(0)', opacity: 0 }),
        animate('0.6s ease-out'),
      ]),
      transition('* => reverse', [
        animate(
          '1.2s ease-in-out',
          keyframes([
            style({ transform: 'translateX(-25px)', opacity: 1, offset: 0.5 }),
            style({ transform: 'translateX(0)', opacity: 1, offset: 1 }),
          ])
        ),
      ]),
    ]),

    trigger('flowRight', [
      state('start', style({ transform: 'translateX(50px)', opacity: 1 })),
      state('reverse', style({ transform: 'translateX(0)', opacity: 1 })),
      transition('* => start', [
        style({ transform: 'translateX(0)', opacity: 0 }),
        animate('0.6s ease-out'),
      ]),
      transition('* => reverse', [
        animate(
          '1.2s ease-in-out',
          keyframes([
            style({ transform: 'translateX(25px)', opacity: 1, offset: 0.5 }),
            style({ transform: 'translateX(0)', opacity: 1, offset: 1 }),
          ])
        ),
      ]),
    ]),

    trigger('mergeItems', [
      state('start', style({ transform: 'scale(1)', opacity: 1 })),
      state('reverse', style({ transform: 'scale(1)', opacity: 1 })),
      transition('start => reverse', [
        animate(
          '1s ease-in-out',
          keyframes([
            style({ transform: 'scale(1.1)', opacity: 0.8, offset: 0.5 }),
            style({ transform: 'scale(1)', opacity: 1, offset: 1 }),
          ])
        ),
      ]),
    ]),
  ],
})
export class SpaceBookingComponent extends Form {
  override form: Formify<ICreateSpaceReservation>;
  @ViewChild('singleDice') singleDice: DiceRollerComponent | undefined;
  @ViewChild('secondDice') secondDice: DiceRollerComponent | undefined;

  public menuItems: BehaviorSubject<IMenuItem[]> = new BehaviorSubject<
    IMenuItem[]
  >([]);
  // public isMenuVisible: boolean = false;
  public timeSlots: string[] = [];
  public activeSlotIndex: number = 0;
  public guestsFirstSection: number = 1;
  public guestsSecondSection: number = 0;
  public isSplit: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly tenantSettingsService: TenantSettingsService,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.tenantSettingsService.get().subscribe({
      next: (result) => {
        if (result) {
          this.timeSlots = result.reservationHours;
        }
      },
    });

    this.menuItems.next([
      { key: 'qr-code', label: 'QR Code' },
      // { key: 'history-log', label: 'Last Activities' },
      { key: 'update', label: 'Update' },
      { key: 'copy', label: 'Add Copy' },
      { key: 'delete', label: 'Delete' },
    ]);
  }

  public get isAddButtonActive(): boolean {
    return this.guestsFirstSection + this.guestsSecondSection === 12;
  }

  public get isMinusButtonActive(): boolean {
    return this.guestsFirstSection === 1;
  }

  public toggleSplit(): void {
    this.isSplit = !this.isSplit;
  }

  public getActiveDiceRoller(): DiceRollerComponent | undefined {
    return this.isSplit ? this.secondDice : this.singleDice;
  }

  public updateGuests(faceValue: number): void {
    if (this.isSplit) {
      this.guestsSecondSection = faceValue;
    } else {
      this.guestsFirstSection = faceValue;
    }
  }

  public onDirectionChange(direction): void {
    if (this.guestsFirstSection === 6 && direction === 'right') {
      this.guestsFirstSection = 6;
      this.isSplit = true;
    } else if (this.guestsSecondSection === 1 && direction === 'left') {
      this.guestsSecondSection = 0;
      this.isSplit = false;
    }
  }

  public onSlotClick(index: number): void {
    this.activeSlotIndex = index;
  }

  public backNavigateBtn(): void {
    this.router.navigateByUrl('space/home');
  }

  public showMenu(event: MouseEvent, controlMenu: ControlsMenuComponent): void {
    event.stopPropagation();
    controlMenu.toggleMenu();
  }

  public bookTable(): void {
    if (this.form.valid) {
      const date: Date = new Date(this.form.controls.reservationDate.value);
      const time: string = this.timeSlots[this.activeSlotIndex];

      const combinedUtc = this.combineDateAndTimeToUtc(date, time);
      this.spaceManagementService
        .bookTable(
          this.guestsFirstSection + this.guestsSecondSection,
          combinedUtc
        )
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesApplied,
              type: ToastType.Success,
            });
            this.router.navigateByUrl('space/home');
          },
          error: (error) => {
            if (
              error.error.detail === 'User already have an active reservation'
            ) {
              this.toastService.error({
                message: 'This account already have an active reservation',
                type: ToastType.Error,
              });
            } else {
              this.handleServerErrors(error);
              this.toastService.error({
                message: AppToastMessage.SomethingWrong,
                type: ToastType.Error,
              });
            }
          },
        });
    }
  }

  private combineDateAndTimeToUtc(date: Date, time: string): Date {
    const [hours, minutes] = time.split(':').map(Number);

    return new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      hours,
      minutes,
      0
    );
  }

  protected override getControlDisplayName(controlName: string): string {
    return controlName;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      reservationDate: new FormControl<Date | null>(null, [
        Validators.required,
      ]),
    });
  }
}
