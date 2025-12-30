import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Form } from '../../../../../shared/components/form/form.component';
import { Formify } from '../../../../../shared/models/form.model';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { IGameDropdownResult } from '../../../../../entities/games/models/game-dropdown.model';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { throwError } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';
import { DatePipe, Location } from '@angular/common';
import { SafeUrl } from '@angular/platform-browser';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../../shared/pipe/entity-image.pipe';
import { DateHelper } from '../../../../../shared/helpers/date-helper';
import { TranslateService } from '@ngx-translate/core';
import { FULL_ROUTE } from '../../../../../shared/configs/route.config';

interface ICreateEventForm {
  name: string;
  description_en: string;
  description_bg: string;
  startDate: string;
  startTime: string;
  maxPeople: number;
  gameId: number;
  image: string | null;
  isCustomImage: boolean;
}

function futureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    const selectedDate = new Date(control.value);
    const today = new Date();

    // Reset times (we only compare dates, not hours)
    today.setHours(0, 0, 0, 0);
    selectedDate.setHours(0, 0, 0, 0);

    return selectedDate > today ? null : { notFutureDate: true };
  };
}

@Component({
  selector: 'app-add-update-event',
  templateUrl: 'add-update-event.component.html',
  styleUrl: 'add-update-event.component.scss',
  standalone: false,
})
export class AddUpdateEventComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<ICreateEventForm>;
  @ViewChild('descAreaEn', { static: false })
  descAreaEn?: ElementRef<HTMLTextAreaElement>;
  @ViewChild('descAreaBg', { static: false })
  descAreaBg?: ElementRef<HTMLTextAreaElement>;

  public editEventId!: number;
  public gameList: IGameDropdownResult[] = [];
  public fileToUpload: File | null = null;
  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;
  public isMenuVisible: boolean = false;
  public currentLang: 'EN' | 'BG' = 'EN';
  private lastValue = '';

  constructor(
    private readonly fb: FormBuilder,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly gameService: GamesService,
    private readonly eventService: EventsService,
    private readonly location: Location,
    private readonly entityImagePipe: EntityImagePipe,
    private readonly cd: ChangeDetectorRef,
    public override readonly toastService: ToastService,
    private readonly datePipe: DatePipe,
    public override translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe((x) => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.activatedRoute.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.editEventId = +id;
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
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

  public ngOnInit(): void {
    this.fetchGameList();

    if (this.editEventId) {
      this.fetchEventById(this.editEventId);
    } else {
      this.initFormValueChanges();
    }
  }

  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public onAdd(): void {
    if (this.form.valid) {
      const startDate = this.form.controls.startDate.value;
      const startTime = this.form.controls.startTime.value;
      const combinedDateTime = DateHelper.combineDateAndTime(
        startDate,
        startTime
      );

      this.eventService
        .add(
          {
            name: this.form.controls.name.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
            gameId: parseInt(this.form.controls.gameId.value as any),
            startDate: combinedDateTime,
            maxPeople: this.form.controls.maxPeople.value,
            isCustomImage: this.form.controls.isCustomImage.value,
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

            this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
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
    if (this.form.valid && this.editEventId) {
      const startDate = this.form.controls.startDate.value;
      const startTime = this.form.controls.startTime.value;
      const combinedDateTime = DateHelper.combineDateAndTime(
        startDate,
        startTime
      );

      this.eventService
        .update(
          {
            id: this.editEventId,
            name: this.form.controls.name.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
            gameId: parseInt(this.form.controls.gameId.value as any),
            startDate: combinedDateTime,
            maxPeople: this.form.controls.maxPeople.value,
            isCustomImage: this.form.controls.isCustomImage.value,
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

            this.router.navigateByUrl(FULL_ROUTE.EVENTS.HOME);
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

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.form.controls.image.patchValue(file.name);
        this.fileToUpload = file;
      };
      reader.readAsDataURL(file);
    } else {
      this.fileToUpload = null;
      this.imagePreview = null;
      this.form.controls.image.reset();
    }
  }

  public backNavigateBtn() {
    this.location.back();
  }

  protected override handleAdditionalErrors(): string | null {
    const startDate = this.form.get('startDate')?.value;

    if (
      startDate &&
      new Date(startDate).getTime() < new Date().setHours(0, 0, 0, 0)
    ) {
      return this.translateService.instant(
        'events.add_update.start_date_validation'
      );
    }

    return null;
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'gameId':
        return this.translateService.instant(
          'events.add_update.controls_display_name.game'
        );
      case 'name':
        return this.translateService.instant(
          'events.add_update.controls_display_name.name'
        );
      case 'description_en':
        return this.translateService.instant(
          'events.add_update.controls_display_name.description_en'
        );
      case 'description_bg':
        return this.translateService.instant(
          'events.add_update.controls_display_name.description_bg'
        );
      case 'startDate':
        return this.translateService.instant(
          'events.add_update.controls_display_name.start_date'
        );
      case 'startTime':
        return this.translateService.instant(
          'events.add_update.controls_display_name.start_time'
        );
      case 'maxPeople':
        return this.translateService.instant(
          'events.add_update.controls_display_name.max_people'
        );
      case 'image':
        return this.translateService.instant(
          'events.add_update.controls_display_name.image'
        );
      default:
        return controlName;
    }
  }

  private fetchEventById(id: number): void {
    this.eventService.getById(id).subscribe({
      next: (event) => {
        if (event) {
          const formattedDate = this.datePipe.transform(
            event.startDate,
            DateHelper.DATE_FORMAT_FOR_INPUT
          );
          const formattedTime = this.datePipe.transform(
            event.startDate,
            DateHelper.TIME_FORMAT
          );
          this.form.patchValue({
            name: event.name,
            description_en: event.description_EN,
            description_bg: event.description_BG,
            gameId: event.gameId,
            startDate: formattedDate?.toString(),
            startTime: formattedTime?.toString(),
            maxPeople: event.maxPeople,
            image: event.imageUrl,
            isCustomImage: event.isCustomImage,
          });
          this.imagePreview = event.imageUrl;

          this.fileToUpload = null;

          this.initFormValueChanges(true);
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
  }

  private fetchGameList(): void {
    this.gameService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
        if (this.form.controls.gameId.value) {
          const selectGame = this.gameList.find(
            (x) => x.id == this.form.controls.gameId.value
          );
          if (selectGame) {
            this.form.patchValue({
              gameId: selectGame.id,
            });
          }
        }
      },
      error: () => {
        this.toastService.error({
          message: this.translateService.instant(
            AppToastMessage.SomethingWrong
          ),
          type: ToastType.Error,
        });
      },
    });
  }

  private fetchGameById(id: number): void {
    this.gameService.getById(id).subscribe({
      next: (game) => {
        if (game) {
          if (!this.form.controls.isCustomImage.value) {
            this.form.patchValue({
              image: game.imageUrl,
            });
            this.imagePreview = game.imageUrl;
            this.cd.detectChanges();

            this.fileToUpload = null;
          }
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  private initFormValueChanges(imageIsPopulated: boolean = false): void {
    this.form.controls.gameId.valueChanges.subscribe((x) => {
      this.fetchGameById(x);
    });

    this.form.controls.isCustomImage.valueChanges.subscribe((x) => {
      if (x && !imageIsPopulated) {
        this.form.patchValue({
          image: '',
        });
        this.imagePreview = null;
        this.fileToUpload = null;
      } else if (!x && this.form.controls.gameId.value) {
        this.fetchGameById(this.form.controls.gameId.value);
      }
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      gameId: new FormControl<number | null>(null, [Validators.required]),
      name: new FormControl<string | null>(null, [
        Validators.required,
        Validators.minLength(3),
      ]),
      description_en: new FormControl<string | null>('', Validators.required),
      description_bg: new FormControl<string | null>('', Validators.required),
      startDate: new FormControl<Date | null>(null, [
        Validators.required,
        futureDateValidator(),
      ]),
      startTime: new FormControl<Date | null>(null, [Validators.required]),
      maxPeople: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
      image: new FormControl<string | null>(null, [Validators.required]),
      isCustomImage: new FormControl<boolean>(false),
    });
  }
}
