import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { GameCategoriesService } from '../../../../../entities/games/api/game-categories.service';
import { IGameCategory } from '../../../../../entities/games/models/game-category.model';
import { Observable, throwError } from 'rxjs';
import { ToastType } from '../../../../../shared/models/toast.model';
import { Form } from '../../../../../shared/components/form/form.component';
import { IGameDropdownResult } from '../../../../../entities/games/models/game-dropdown.model';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { Formify } from '../../../../../shared/models/form.model';
import { GameQrCodeDialog } from '../../../dialogs/qr-code-dialog/qr-code-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SafeUrl } from '@angular/platform-browser';
import { GameAveragePlaytime } from '../../../../../entities/games/enums/game-average-playtime.enum';
import { QrCodeType } from '../../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../../shared/pipe/entity-image.pipe';
import { IDropdown } from '../../../../../shared/models/dropdown.model';

interface ICreateGameForm {
  categoryId: number;
  name: string;
  description: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
  image: string;
}

@Component({
  selector: 'app-add-update-game',
  templateUrl: 'add-update-game.component.html',
  styleUrl: 'add-update-game.component.scss',
})
export class AddUpdateGameComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<ICreateGameForm>;
  @ViewChild('descArea') descArea!: ElementRef<HTMLTextAreaElement>;
  public categories!: Observable<IGameCategory[] | null>;
  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;
  public showQRCode = false;
  public editGameId: number | null = null;
  public editGameName: string | null = null;
  public gameList: IGameDropdownResult[] = [];
  public selectedGame: IGameDropdownResult | null = null;
  public addExistingGame: boolean = false;
  public imageError: string | null = null;
  public fileToUpload: File | null = null;
  public gamAveragePlaytimeValues: IDropdown[] = [];

  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly gameCategoriesService: GameCategoriesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly activatedRoute: ActivatedRoute,
    private readonly entityImagePipe: EntityImagePipe,
    public override readonly toastService: ToastService
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);

    this.gamAveragePlaytimeValues = Object.entries(GameAveragePlaytime)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: value.toString() }));
  }
  
  public adjustTextareaHeight(textarea: HTMLTextAreaElement): void {
    textarea.style.height = 'auto'; // Reset height
    textarea.style.height = textarea.scrollHeight + 'px'; // Fit to content
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }
  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public ngOnInit(): void {
    this.categories = this.gameCategoriesService.getList();
    if (
      this.activatedRoute.snapshot.routeConfig?.path?.includes(
        'add-existing-game'
      )
    ) {
      this.preparationForAddExistingGame();
    } else {
      this.activatedRoute.paramMap.subscribe((params) => {
        const id = params.get('id');
        if (id) {
          this.editGameId = +id;
          this.fetchGameById(this.editGameId);
        }
      });
    }
  }

  public openQrCodeDialog(): void {
    this.dialog.open(GameQrCodeDialog, {
      width: '17rem',
      data: {
        Id: this.editGameId,
        Name: this.editGameName,
        Type: QrCodeType.Game,
      },
    });
  }

  public preparationForAddExistingGame() {
    this.addExistingGame = true;
    this.fetchGameList();
    this.activatedRoute.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.selectedGame = { id: +id, name: '' };
        this.handleAddExistingGame();
      }
    });
    this.form.controls.categoryId.disable();
    this.form.controls.name.disable();
    this.form.controls.description.disable();
    this.form.controls.averagePlaytime.disable();
    this.form.controls.minAge.disable();
    this.form.controls.maxPlayers.disable();
    this.form.controls.minPlayers.disable();
  }
  public handleAddExistingGame(): void {
    if (this.selectedGame?.id) {
      this.fetchGameById(this.selectedGame.id);
    }
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('games/library');
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    console.log(input.files);

    const file = input.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.form.controls.image.patchValue(file.name);
        this.fileToUpload = file;
        this.imageError = null;
        console.log(this.form.controls);
      };
      reader.readAsDataURL(file);
    } else {
      this.imageError = 'Image is required.';
      this.fileToUpload = null;
      this.imagePreview = null;
      this.form.controls.image.reset();
    }
  }

  public onAdd(): void {
    if (!this.fileToUpload) {
      this.imageError = 'Image is required.';
      return;
    }
    if (this.form.valid && this.fileToUpload) {
      this.gameService
        .add(
          {
            categoryId: parseInt(this.form.controls.categoryId.value as any),
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            minAge: this.form.controls.minAge.value,
            minPlayers: this.form.controls.minPlayers.value,
            maxPlayers: this.form.controls.maxPlayers.value,
            averagePlaytime: this.form.controls.averagePlaytime.value,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            //TODO: Just an idea maybe show qr after redirect from creating ?
            this.router.navigateByUrl('/games/library');
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

  public onUpdate(): void {
    if (this.editGameId) {
      this.gameService
        .update(
          {
            id: this.editGameId,
            categoryId: parseInt(this.form.controls.categoryId.value as any),
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            minAge: this.form.controls.minAge.value,
            minPlayers: this.form.controls.minPlayers.value,
            maxPlayers: this.form.controls.maxPlayers.value,
            averagePlaytime: this.form.controls.averagePlaytime.value,
            imageId: !this.fileToUpload
              ? +this.form.controls.image.value
              : null,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesApplied,
              type: ToastType.Success,
            });

            this.router.navigateByUrl(`/games/${this.editGameId}/details`);
          },
          error: (error) => {
            console.log('errors--->', error);
            this.handleServerErrors(error);
            this.toastService.error({
              message: AppToastMessage.FailedToSaveChanges,
              type: ToastType.Error,
            });
          },
        });
    }
  }

  public onCopy(): void {
    if (this.addExistingGame && this.selectedGame) {
      this.gameService.addCopy(this.selectedGame.id).subscribe({
        next: (_) => {
          this.toastService.success({
            message: AppToastMessage.ChangesSaved,
            type: ToastType.Success,
          });
          this.router.navigateByUrl(`/games/library`);
        },
        error: (error) => {
          this.handleServerErrors(error);
          this.toastService.error({
            message: AppToastMessage.FailedToSaveChanges,
            type: ToastType.Error,
          });
        },
      });
      return;
    }
  }

  protected override handleAdditionalErrors(): string | null {
    return this.imageError ? this.imageError : null;
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'categoryId':
        return 'Category';
      case 'name':
        return 'Name';
      case 'description':
        return 'Description';
      case 'minAge':
        return 'Minimum Age';
      case 'minPlayers':
        return 'Minimum Players';
      case 'maxPlayers':
        return 'Maximum Players';
      case 'averagePlaytime':
        return 'Average Playtime';
      case 'image':
        return 'Image';
      default:
        return controlName;
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
        console.log(error);
      },
    });
  }

  private fetchGameById(id: number): void {
    this.gameService.getById(id).subscribe({
      next: (game) => {
        if (game) {
          this.form.patchValue({
            categoryId: game.categoryId,
            name: game.name,
            description: game.description,
            minAge: game.minAge,
            minPlayers: game.minPlayers,
            maxPlayers: game.maxPlayers,
            averagePlaytime: game.averagePlaytime,
            image: game.imageId.toString(),
          });
          this.editGameName = game.name;

          this.entityImagePipe
            .transform(ImageEntityType.Games, game.imageId)
            .subscribe((image) => (this.imagePreview = image));

          this.fileToUpload = null;

          this.adjustTextareaHeight(this.descArea.nativeElement);
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      categoryId: new FormControl<number | null>(null, [Validators.required]),
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      description: new FormControl<string | null>('', Validators.required),
      minAge: new FormControl<number>(12, [Validators.required]),
      minPlayers: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
      maxPlayers: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
      averagePlaytime: new FormControl<number>(5, [Validators.required]),
      image: new FormControl<string | null>('', [Validators.required]),
    });
  }
}
