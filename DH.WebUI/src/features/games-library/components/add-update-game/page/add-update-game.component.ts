import { TranslateService } from '@ngx-translate/core';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { ActivatedRoute } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { GameCategoriesService } from '../../../../../entities/games/api/game-categories.service';
import { IGameCategory } from '../../../../../entities/games/models/game-category.model';
import { Observable, throwError } from 'rxjs';
import { ToastType } from '../../../../../shared/models/toast.model';
import { Form } from '../../../../../shared/components/form/form.component';
import { IGameDropdownResult } from '../../../../../entities/games/models/game-dropdown.model';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { Formify } from '../../../../../shared/models/form.model';
import { QrCodeDialog } from '../../../dialogs/qr-code-dialog/qr-code-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SafeUrl } from '@angular/platform-browser';
import { GameAveragePlaytime } from '../../../../../entities/games/enums/game-average-playtime.enum';
import { QrCodeType } from '../../../../../entities/qr-code-scanner/enums/qr-code-type.enum';
import { IDropdown } from '../../../../../shared/models/dropdown.model';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';
import { TenantRouter } from '../../../../../shared/helpers/tenant-router';

interface ICreateGameForm {
  categoryId: number;
  name: string;
  description_en: string;
  description_bg: string;
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
  standalone: false,
})
export class AddUpdateGameComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<ICreateGameForm>;
  @ViewChild('descAreaEn', { static: false })
  descAreaEn?: ElementRef<HTMLTextAreaElement>;
  @ViewChild('descAreaBg', { static: false })
  descAreaBg?: ElementRef<HTMLTextAreaElement>;
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
  public currentLang: 'EN' | 'BG' = 'EN';
  private lastValue = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly gameCategoriesService: GameCategoriesService,
    private readonly menuTabsService: MenuTabsService,
    private readonly tenantRouter: TenantRouter,
    private readonly dialog: MatDialog,
    private readonly activatedRoute: ActivatedRoute,
    public override readonly toastService: ToastService,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
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

  public ngAfterViewChecked(): void {
    this.adjustCurrentTextareaHeight();
  }

  public adjustCurrentTextareaHeight(): void {
    const textarea =
      this.currentLang === 'EN'
        ? this.descAreaEn?.nativeElement
        : this.descAreaBg?.nativeElement;

    if (textarea && textarea.value !== this.lastValue) {
      this.lastValue = textarea.value;
      this.adjustTextareaHeight(textarea);
    }
  }

  public adjustTextareaHeight(textarea: HTMLTextAreaElement): void {
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }

  public setLang(lang: 'EN' | 'BG') {
    this.currentLang = lang;

    setTimeout(() => {
      this.adjustCurrentTextareaHeight();
    });
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
    this.dialog.open(QrCodeDialog, {
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
    this.form.controls.description_en.disable();
    this.form.controls.description_bg.disable();
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
    this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
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
      this.imageError = this.translateService.instant(
        'games.game.add_update.controls_display_name.image_required'
      );
      this.fileToUpload = null;
      this.imagePreview = null;
      this.form.controls.image.reset();
    }
  }

  public onAdd(): void {
    if (!this.fileToUpload) {
      this.imageError = this.translateService.instant(
        'games.game.add_update.controls_display_name.image_required'
      );
      return;
    }
    if (this.form.valid && this.fileToUpload) {
      this.gameService
        .add(
          {
            categoryId: parseInt(this.form.controls.categoryId.value as any),
            name: this.form.controls.name.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
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
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });

            this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
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

  public onUpdate(): void {
    if (this.editGameId) {
      this.gameService
        .update(
          {
            id: this.editGameId,
            categoryId: parseInt(this.form.controls.categoryId.value as any),
            name: this.form.controls.name.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
            minAge: this.form.controls.minAge.value,
            minPlayers: this.form.controls.minPlayers.value,
            maxPlayers: this.form.controls.maxPlayers.value,
            averagePlaytime: this.form.controls.averagePlaytime.value,
            imageUrl: !this.fileToUpload
              ? this.form.controls.image.value
              : null,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesApplied
              ),
              type: ToastType.Success,
            });

            if (this.editGameId)
              this.tenantRouter.navigateTenant(
                FULL_ROUTE.GAMES.DETAILS(this.editGameId)
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
    }
  }

  public onCopy(): void {
    if (this.addExistingGame && this.selectedGame) {
      this.gameService.addCopy(this.selectedGame.id).subscribe({
        next: (_) => {
          this.toastService.success({
            message: this.translateService.instant(
              AppToastMessage.ChangesSaved
            ),
            type: ToastType.Success,
          });
          this.tenantRouter.navigateTenant(FULL_ROUTE.GAMES.LIBRARY);
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
      return;
    }
  }

  protected override handleAdditionalErrors(): string | null {
    return this.imageError ? this.imageError : null;
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'categoryId':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.category'
        );
      case 'name':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.name'
        );
      case 'description_en':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.description_en'
        );
      case 'description_bg':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.description_bg'
        );
      case 'minAge':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.min_age'
        );
      case 'minPlayers':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.min_players'
        );
      case 'maxPlayers':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.max_players'
        );
      case 'averagePlaytime':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.average_playtime'
        );
      case 'image':
        return this.translateService.instant(
          'games.game.add_update.controls_display_name.image'
        );
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
            description_en: game.description_EN,
            description_bg: game.description_BG,
            minAge: game.minAge,
            minPlayers: game.minPlayers,
            maxPlayers: game.maxPlayers,
            averagePlaytime: game.averagePlaytime,
            image: game.imageUrl,
          });
          this.editGameName = game.name;

          this.imagePreview = game.imageUrl;

          this.fileToUpload = null;

          setTimeout(() => {
            this.adjustCurrentTextareaHeight();
          });
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
      description_en: new FormControl<string | null>('', Validators.required),
      description_bg: new FormControl<string | null>('', Validators.required),
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
