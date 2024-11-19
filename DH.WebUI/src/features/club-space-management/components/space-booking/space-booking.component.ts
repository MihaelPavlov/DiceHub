import { AfterViewInit, Component } from '@angular/core';
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
interface ICreateSpaceReservation {
  gameName: string;
  gameId: number;
  tableName: string;
  maxPeople: number;
  password: string;
}
@Component({
  selector: 'app-club-space-booking',
  templateUrl: 'space-booking.component.html',
  styleUrl: 'space-booking.component.scss',
})
export class SpaceBookingComponent extends Form implements AfterViewInit {
  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }
  override form: Formify<ICreateSpaceReservation>;

  public isMenuVisible: boolean = false;
  timeSlots: string[] = ['17:30', '18:00', '18:30', '19:00', '19:30', '20:00'];
  activeSlotIndex: number | null = null; // Holds the index of the active slot

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly loadingService: LoadingService
  ) {
    super(toastService);
    this.loadingService.loadingOn();
    this.form = this.initFormGroup();
  }
  ngAfterViewInit(): void {
    this.loadingService.loadingOff();
  }

  numberOfGuests: number = 1; // Default number of guests

  public onAdd() {
    this.numberOfGuests = Math.min(this.numberOfGuests + 1, 6); // Ensure it doesn't exceed 6
  }

  public onRemove() {
    this.numberOfGuests = Math.max(this.numberOfGuests - 1, 1); // Ensure it doesn't go below 1
  }

  public updateGuests(faceValue: number): void {
    this.numberOfGuests = faceValue; // Sync value from child
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
  private initFormGroup(): FormGroup {
    return this.fb.group({
      numberGuests: new FormControl<number>(1, [Validators.required]),
      reservationDate: new FormControl<Date | null>(null, [
        Validators.required,
      ]),
    });
  }
}
