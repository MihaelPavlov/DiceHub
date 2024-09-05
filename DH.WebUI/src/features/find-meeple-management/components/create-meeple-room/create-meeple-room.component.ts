import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import { ICreateRoomDto } from '../../../../entities/rooms/models/create-room.model';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { combineLatest, throwError } from 'rxjs';
import { IGameByIdResult } from '../../../../entities/games/models/game-by-id.model';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { Router } from '@angular/router';
import { IGameInventory } from '../../../../entities/games/models/game-inventory.mode';
import { GameImagePipe } from '../../../../shared/pipe/game-image.pipe';

export interface ICreateRoomForm {
  name: string;
  startDate: string;
  startTime: string;
  maxParticipants: number;
  gameId: number;
}

@Component({
  selector: 'app-create-meeple-room',
  templateUrl: 'create-meeple-room.component.html',
  styleUrl: 'create-meeple-room.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class CreateMeepleRoomComponent
  extends Form
  implements OnInit, OnDestroy
{
  override form: Formify<ICreateRoomForm>;
  public gameList: IGameDropdownResult[] = [];
  public selectedGame: IGameDropdownResult | null = null;
  public game: IGameByIdResult | null = null;
  public gameInventory!: IGameInventory;
  public imagePreview: string | ArrayBuffer | null = null;
  public isMenuVisible: boolean = false;

  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly roomService: RoomsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly fb: FormBuilder,
    private readonly gameImagePipe: GameImagePipe,
    private readonly router: Router
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.MEEPLE);
  }

  public getGame(prop: keyof IGameByIdResult): string {
    if (!this.game || this.game[prop] === undefined) {
      return '';
    }
    return this.game[prop].toString();
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  public ngOnInit(): void {
    this.fetchGameList();
  }

  public handleAddExistingGame(): void {
    if (this.selectedGame?.id) {
      this.fetchGameById(this.selectedGame.id);
    }
  }
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }
  public backNavigateBtn() {
    this.router.navigateByUrl('meeples/find');
  }

  public onSubmit(): void {
    const startDate = this.form.controls.startDate.value;
    const startTime = this.form.controls.startTime.value;
    if (this.form.valid && startDate && startTime) {
      const combinedDateTime = this.combineDateAndTime(startDate, startTime);

      this.roomService
        .add({
          gameId: parseInt(this.form.controls.gameId.value as any),
          name: this.form.controls.name.value,
          startDate: combinedDateTime,
          maxParticipants: this.form.controls.maxParticipants.value,
        })
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.router.navigateByUrl('/meeples/find');
          },
          error: (error) => {
            this.handleServerErrors(error);
            this.toastService.error({
              message: AppToastMessage.FailedToSaveChanges,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  private fetchGameList(): void {
    this.gameService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
        if (this.selectedGame) {
          const selectGame = this.gameList.find(
            (x) => x.id == this.selectedGame?.id
          );
          if (selectGame) this.selectedGame = selectGame;
        }
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
        throwError(() => error);
      },
    });
  }

  private fetchGameById(id: number): void {
    combineLatest([
      this.gameService.getById(id),
      this.gameService.getInventory(id),
    ]).subscribe({
      next: ([game, inventory]) => {
        if (game && inventory) {
          this.game = game;
          this.form.patchValue({
            gameId: game.id,
          });
          this.imagePreview = this.gameImagePipe.transform(game.imageId);

          this.gameInventory = inventory;
        }
      },
      error: (error) => {
        this.toastService.error({
          message: AppToastMessage.SomethingWrong,
          type: ToastType.Error,
        });
        throwError(() => error);
      },
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'name':
        return 'Name';
      case 'startDate':
        return 'Start date';
      case 'startTime':
        return 'Start time';
      case 'maxParticipants':
        return 'Max participants';
      case 'gameId':
        return 'Game';
      default:
        return controlName;
    }
  }

  private combineDateAndTime(date: string, time: string): string {
    const parsedDate: string = new Date(`${date}T${time}:00`).toISOString();
    return parsedDate;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      gameId: new FormControl<number | null>(null, [Validators.required]),
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      startDate: new FormControl<Date | null>(null, [Validators.required]),
      startTime: new FormControl<Date | null>(null, [Validators.required]),
      maxParticipants: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }
}
