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
import { throwError } from 'rxjs';
import { IGameByIdResult } from '../../../../entities/games/models/game-by-id.model';

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
  override form: Formify<ICreateRoomDto>;
  public gameList: IGameDropdownResult[] = [];
  public selectedGame: IGameDropdownResult | null = null;
  public game: IGameByIdResult | null = null;
  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly roomService: RoomsService,
    private readonly menuTabsService: MenuTabsService,
    private readonly fb: FormBuilder
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

  public onSubmit(): void {
    console.log(this.form.controls);
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
        console.log(error);
      },
    });
  }

  private fetchGameById(id: number): void {
    this.gameService.getById(id).subscribe({
      next: (game) => {
        if (game) {
          this.game = game;
          // this.form.patchValue({
          //   categoryId: game.categoryId,
          //   name: game.name,
          //   description: game.description,
          //   minAge: game.minAge,
          //   minPlayers: game.minPlayers,
          //   maxPlayers: game.maxPlayers,
          //   averagePlaytime: game.averagePlaytime,
          //   image: game.imageId.toString(),
          // });
          // this.imagePreview = `https://localhost:7024/games/get-image/${game.imageId}`;
          // this.fileToUpload = null;
        }
      },
      error: (error) => {
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

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }
}
