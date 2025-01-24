import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
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
import { ActivatedRoute, Router } from '@angular/router';
import { IGameInventory } from '../../../../entities/games/models/game-inventory.mode';
import { SafeUrl } from '@angular/platform-browser';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../shared/pipe/entity-image.pipe';
import { DateHelper } from '../../../../shared/helpers/date-helper';
import { DatePipe } from '@angular/common';

interface IAddUpdateRoomForm {
  name: string;
  startDate: string;
  startTime: string;
  maxParticipants: number;
  gameId: number;
}

@Component({
  selector: 'app-add-update-meeple-room',
  templateUrl: 'add-update-meeple-room.component.html',
  styleUrl: 'add-update-meeple-room.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class AddUpdateMeepleRoomComponent
  extends Form
  implements OnInit, OnDestroy
{
  override form: Formify<IAddUpdateRoomForm>;
  public gameList: IGameDropdownResult[] = [];
  public selectedGame: IGameDropdownResult | null = null;
  public game: IGameByIdResult | null = null;
  public gameInventory!: IGameInventory;
  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;
  public isMenuVisible: boolean = false;
  public editRoomId: number | null = null;

  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly roomService: RoomsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly fb: FormBuilder,
    private readonly entityImagePipe: EntityImagePipe,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly datePipe: DatePipe
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

    this.activatedRoute.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.editRoomId = +id;
        this.fetchRoom(this.editRoomId);
      }
    });
  }

  public handleSelectGame(): void {
    if (this.selectedGame?.id) {
      this.fetchGameById(this.selectedGame.id);
    }
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public backNavigateBtn() {
    if (this.editRoomId)
      this.router.navigateByUrl(`meeples/${this.editRoomId}/details`);
    else this.router.navigateByUrl('meeples/find');
  }

  public onSubmit(): void {
    const startDate = this.form.controls.startDate.value;
    const startTime = this.form.controls.startTime.value;
    if (this.form.valid && startDate && startTime) {
      const combinedDateTime = DateHelper.combineDateAndTime(
        startDate,
        startTime
      );

      if (this.editRoomId) {
        this.roomService
          .update({
            id: this.editRoomId,
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

              this.router.navigateByUrl(`/meeples/${this.editRoomId}/details`);
            },
            error: (error) => {
              this.handleServerErrors(error);
              this.toastService.error({
                message: AppToastMessage.FailedToSaveChanges,
                type: ToastType.Error,
              });
            },
          });
      } else {
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
  }

  private fetchRoom(id: number): void {
    this.roomService.getById(id).subscribe({
      next: (room) => {
        console.log(room);

        const formattedDate = this.datePipe.transform(
          room.startDate,
          DateHelper.DATE_FORMAT
        );
        const formattedTime = this.datePipe.transform(
          room.startDate,
          DateHelper.TIME_FORMAT
        );
        this.form.patchValue({
          gameId: parseInt(room.gameId as any),
          name: room.name,
          startDate: formattedDate?.toString(),
          startTime: formattedTime?.toString(),
          maxParticipants: room.maxParticipants,
        });

        const selectGame = this.gameList.find((x) => x.id == room.gameId);
        if (selectGame) {
          this.selectedGame = selectGame;
          this.handleSelectGame();
        }
      },
    });
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
          this.entityImagePipe
            .transform(ImageEntityType.Games, game.imageId)
            .subscribe((image) => (this.imagePreview = image));

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

  private clearServerErrorMessage(): void {
    this.getServerErrorMessage = null;
  }
}
