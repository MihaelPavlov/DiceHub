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

@Component({
  selector: 'app-game-availability',
  templateUrl: 'game-availability.component.html',
  styleUrl: 'game-availability.component.scss',
})
export class GameAvailabilityComponent implements OnInit, OnDestroy {
  public game$!: Observable<IGameByIdResult>;
  public gameId!: number;
  public gameInventory$!: Observable<IGameInventory>;
  public gameReservationStatus: IGameReservationStatus | null = null;

  public availableMinutes = [1, 2, 5, 10, 15, 20, 30, 40, 50, 60];
  public currentTimer = 15;
  display: string = '';

  constructor(
    private readonly gameService: GamesService,
    private readonly activeRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly menuTabsService: MenuTabsService,
    private readonly toastService: ToastService
  ) {
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
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
    this.gameService
      .reservation({ gameId, durationInMinutes: this.currentTimer })
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
          const errorMessage = error.error.errors['reservationExist'[0]];
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
}
