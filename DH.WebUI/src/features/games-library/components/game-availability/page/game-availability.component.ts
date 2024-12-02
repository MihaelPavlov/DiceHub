import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { Observable } from 'rxjs';
import { IGameByIdResult } from '../../../../../entities/games/models/game-by-id.model';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { IGameInventory } from '../../../../../entities/games/models/game-inventory.mode';
import { IGameReservationStatus } from '../../../../../entities/games/models/game-reservation-status.model';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ToastType } from '../../../../../shared/models/toast.model';
import { Form } from '../../../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Formify } from '../../../../../shared/models/form.model';
import { MatDialog } from '@angular/material/dialog';
import { QrCodeType } from '../../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { AuthService } from '../../../../../entities/auth/auth.service';
import { ReservationQrCodeDialog } from '../../../../../shared/dialogs/reservation-qr-code/reservation-qr-code.component';

interface IReservationGameForm {
  reservationPeopleCount: number;
  reservationInMinutes: number;
}

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'app-game-availability',
  templateUrl: 'game-availability.component.html',
  styleUrl: 'game-availability.component.scss',
})
export class GameAvailabilityComponent
  extends Form
  implements OnInit, OnDestroy
{
  override form: Formify<IReservationGameForm>;
  public game$!: Observable<IGameByIdResult>;
  public gameId!: number;
  public gameInventory$!: Observable<IGameInventory>;
  public gameReservationStatus: IGameReservationStatus | null = null;

  public availableMinutes = [1, 2, 5, 10, 15, 20, 30, 40, 50, 60];
  public currentTimer = 15;
  display: string = '';
  public peopleNumber: IDropdown[] = [];
  public reservationMinutes: IDropdown[] = [];

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly menuTabsService: MenuTabsService,
    public override readonly toastService: ToastService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog
  ) {
    super(toastService);
    this.form = this.initFormGroup();

    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);

    this.peopleNumber = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((key) => ({
      id: key as number,
      name: key.toString(),
    }));
    this.reservationMinutes = [1, 2, 5, 10, 15, 20, 30, 40, 50, 60].map(
      (key) => ({ id: key as number, name: key.toString() })
    );
  }

  public ngOnInit(): void {
    this.activeRoute.params.subscribe((params: Params) => {
      const gameId = params['id'];
      this.gameId = gameId;
      this.game$ = this.gameService.getById(gameId);
      this.gameInventory$ = this.gameService.getInventory(gameId);

      this.fetchReservationStatus(gameId);
    });
  }

  public openDialog(
    id: number,
  ) {

    const dialogRef = this.dialog.open(ReservationQrCodeDialog, {
      width: '17rem',
      data: {
        Id : id,
        Name: 'GameReservation',
        Type: QrCodeType.GameReservation,
        AdditionalData: {
          "userId": this.authService.getUser?.id,
        },
      },
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'reservationPeopleCount':
        return 'People';
      case 'reservationInMinutes':
        return 'Arrive Time';
      default:
        return controlName;
    }
  }

  private fetchReservationStatus(gameId: number): void {
    this.gameService.reservationStatus(gameId).subscribe({
      next: (status: IGameReservationStatus | null) => {
        this.gameReservationStatus = status;
        console.log('status of reservation', status);

        if (status) {
          const secondsLeft = this.calculateSecondsLeft(
            new Date(status.reservationDate),
            status.reservedDurationMinutes
          );

          if (secondsLeft > 0) {
            this.startTimer(secondsLeft);
          }
        }
      },
      error: () => {
        this.gameReservationStatus = null;
      },
    });
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public navigateBackToGameList(): void {
    this.router.navigate(['games/library']);
  }

  public decreaseTimer() {
    const currentIndex = this.availableMinutes.indexOf(this.currentTimer);
    if (currentIndex > 0) {
      this.currentTimer = this.availableMinutes[currentIndex - 1];
    }
  }

  public increaseTimer() {
    const currentIndex = this.availableMinutes.indexOf(this.currentTimer);
    if (currentIndex < this.availableMinutes.length - 1) {
      this.currentTimer = this.availableMinutes[currentIndex + 1];
    }
  }

  public onReservation(gameId: number): void {
    if (this.form.valid) {
      this.gameService
        .reservation({
          gameId,
          durationInMinutes: this.form.controls.reservationInMinutes.value,
          peopleCount: this.form.controls.reservationPeopleCount.value,
        })
        .subscribe({
          next: (x) => {
            this.gameInventory$ = this.gameService.getInventory(gameId);
            this.toastService.success({
              message: 'Reservation is successfully',
              type: ToastType.Success,
            });

            this.fetchReservationStatus(gameId);
          },
          error: (error) => {
            const errorMessage = error.error.errors['reservationExist'][0];

            if (errorMessage) {
              this.toastService.error({
                message: errorMessage,
                type: ToastType.Error,
              });
            } else {
              this.toastService.error({
                message: 'Reservation was not successfully',
                type: ToastType.Error,
              });
            }
          },
        });
    }
  }

  private calculateSecondsLeft(
    reservationDate: Date,
    reservedDurationMinutes: number
  ): number {
    const currentTime = Date.now();

    const reservationEndTime =
      reservationDate.getTime() + reservedDurationMinutes * 60000;

    const differenceInMs = reservationEndTime - currentTime;
    const secondsLeft = Math.floor(differenceInMs / 1000);

    return Math.max(secondsLeft, 0);
  }

  private startTimer(secondsLeft: number): void {
    let seconds = secondsLeft;

    const timer = setInterval(() => {
      const minutes = Math.floor(seconds / 60);
      const displaySeconds = seconds % 60;

      const formattedMinutes = minutes < 10 ? '0' + minutes : minutes;
      const formattedSeconds =
        displaySeconds < 10 ? '0' + displaySeconds : displaySeconds;

      this.display = `${formattedMinutes}:${formattedSeconds}`;

      seconds--;

      if (seconds < 0) {
        clearInterval(timer);
        this.display = '00:00';
        this.fetchReservationStatus(this.gameId);
      }
    }, 1000);
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      reservationPeopleCount: new FormControl<number | null>(2, [
        Validators.required,
      ]),
      reservationInMinutes: new FormControl<number>(15, [Validators.required]),
    });
  }
}
