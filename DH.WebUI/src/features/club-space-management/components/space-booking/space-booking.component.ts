import { TenantSettingsService } from './../../../../entities/common/api/tenant-settings.service';
import { SpaceManagementService } from './../../../../entities/space-management/api/space-management.service';
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingService } from '../../../../shared/services/loading.service';
import { Formify } from '../../../../shared/models/form.model';
import { Form } from '../../../../shared/components/form/form.component';
import { ToastService } from '../../../../shared/services/toast.service';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  animate,
  keyframes,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { DiceRollerComponent } from './components/dice-scroller/dice-roller.component';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

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
export class SpaceBookingComponent extends Form implements AfterViewInit {
  override form: Formify<ICreateSpaceReservation>;
  @ViewChild('singleDice') singleDice: DiceRollerComponent | undefined;
  @ViewChild('secondDice') secondDice: DiceRollerComponent | undefined;

  public isMenuVisible: boolean = false;
  public timeSlots: string[] = [];
  public activeSlotIndex: number = 0;
  public guestsFirstSection: number = 1;
  public guestsSecondSection: number = 0;
  public isSplit: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly loadingService: LoadingService,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly tenantSettingsService: TenantSettingsService
  ) {
    super(toastService);
    this.loadingService.loadingOn();
    this.form = this.initFormGroup();
    this.tenantSettingsService.get().subscribe({
      next:(result)=>{
        if(result){
          this.timeSlots = result.reservationHours;
        }
      }
    })
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
  public ngAfterViewInit(): void {
    this.loadingService.loadingOff();
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

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public bookTable(): void {
    if (this.form.valid) {
      this.spaceManagementService
        .bookTable(
          this.guestsFirstSection + this.guestsSecondSection,
          this.form.controls.reservationDate.value,
          this.timeSlots[this.activeSlotIndex]
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
