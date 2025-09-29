import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import { GamesService } from '../../../../entities/games/api/games.service';
import { RoomsService } from '../../../../entities/rooms/api/rooms.service';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
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
import { TranslateService } from '@ngx-translate/core';
import { FULL_ROUTE } from '../../../../shared/configs/route.config';
import {
  ImagePreviewDialog,
  ImagePreviewData,
} from '../../../../shared/dialogs/image-preview/image-preview.dialog';
import { MatDialog } from '@angular/material/dialog';

interface IAddUpdateRoomForm {
  name: string;
  startDate: string;
  startTime: string;
  maxParticipants: number;
  gameId: number;
}

function futureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    const selectedDate = new Date(control.value);
    const today = new Date();

    // Reset times (we only compare dates, not hours)
    today.setHours(0, 0, 0, 0);
    selectedDate.setHours(0, 0, 0, 0);

    return selectedDate > today ? null : { cannotBeToday: true };
  };
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
  public editRoomId: number | null = null;
  public readonly ImageEntityType = ImageEntityType;

  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly roomService: RoomsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly fb: FormBuilder,
    private readonly entityImagePipe: EntityImagePipe,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly datePipe: DatePipe,
    public override translateService: TranslateService,
    private readonly dialog: MatDialog
  ) {
    super(toastService, translateService);
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
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      this.editRoomId = +id;
      this.fetchRoom(this.editRoomId);
    } else {
      this.fetchGameList();
    }
  }

  public handleSelectGame(): void {
    if (this.selectedGame?.id) {
      this.fetchGameById(this.selectedGame.id);
    }
  }

  public openImagePreview(imageUrl: string) {
    this.dialog.open<ImagePreviewDialog, ImagePreviewData>(ImagePreviewDialog, {
      data: {
        imageUrl,
        title: this.translateService.instant('image'),
      },
      width: '17rem',
    });
  }

  public backNavigateBtn() {
    if (this.editRoomId)
      this.router.navigateByUrl(
        FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(this.editRoomId)
      );
    else this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.FIND);
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
                message: this.translateService.instant(
                  AppToastMessage.ChangesSaved
                ),
                type: ToastType.Success,
              });

              if (this.editRoomId)
                this.router.navigateByUrl(
                  FULL_ROUTE.MEEPLE_ROOM.DETAILS_BY_ID(this.editRoomId)
                );
            },
            error: (error) => {
              this.handleServerErrors(error);
              this.toastService.error({
                message: this.translateService.instant(
                  AppToastMessage.FailedToSaveChanges
                ),
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
                message: this.translateService.instant(
                  AppToastMessage.ChangesSaved
                ),
                type: ToastType.Success,
              });

              this.router.navigateByUrl(FULL_ROUTE.MEEPLE_ROOM.FIND);
            },
            error: (error) => {
              this.handleServerErrors(error);
              this.toastService.error({
                message: this.translateService.instant(
                  AppToastMessage.FailedToSaveChanges
                ),
                type: ToastType.Error,
              });
            },
          });
      }
    }
  }

  private fetchRoom(id: number): void {
    combineLatest([
      this.gameService.getDropdownList(),
      this.roomService.getById(id),
    ]).subscribe({
      next: ([gameList, room]) => {
        console.log(room);
        this.gameList = gameList;
        const formattedDate = this.datePipe.transform(
          room.startDate,
          DateHelper.DATE_FORMAT_FOR_INPUT
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
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
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
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
        throwError(() => error);
      },
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'name':
        return this.translateService.instant(
          'meeple.add_update.controls_display_name.name'
        );
      case 'startDate':
        return this.translateService.instant(
          'meeple.add_update.controls_display_name.start_date'
        );
      case 'startTime':
        return this.translateService.instant(
          'meeple.add_update.controls_display_name.start_time'
        );
      case 'maxParticipants':
        return this.translateService.instant(
          'meeple.add_update.controls_display_name.max_participants'
        );
      case 'gameId':
        return this.translateService.instant(
          'meeple.add_update.controls_display_name.game'
        );
      default:
        return controlName;
    }
  }

  protected override handleAdditionalErrors(): string | null {
    const startDate = this.form.get('startDate')?.value;

    if (
      startDate &&
      new Date(startDate).getTime() < new Date().setHours(0, 0, 0, 0)
    ) {
      return this.translateService.instant(
        'meeple.add_update.validation_message_cannot_be_today'
      );
    }

    return null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      gameId: new FormControl<number | null>(null, [Validators.required]),
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      startDate: new FormControl<Date | null>(null, [
        Validators.required,
        futureDateValidator(),
      ]),
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
