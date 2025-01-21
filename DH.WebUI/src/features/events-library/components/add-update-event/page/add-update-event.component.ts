import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Form } from '../../../../../shared/components/form/form.component';
import { Formify } from '../../../../../shared/models/form.model';
import {
  FormBuilder,
  FormControl,
  FormGroup,
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
import { Location } from '@angular/common';
import { SafeUrl } from '@angular/platform-browser';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../../shared/pipe/entity-image.pipe';

interface ICreateEventForm {
  name: string;
  description: string;
  startDate: Date;
  maxPeople: number;
  gameId: number;
  image: string | null;
  isCustomImage: boolean;
}

@Component({
  selector: 'app-add-update-event',
  templateUrl: 'add-update-event.component.html',
  styleUrl: 'add-update-event.component.scss',
})
export class AddUpdateEventComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<ICreateEventForm>;
  public editEventId!: number;
  public gameList: IGameDropdownResult[] = [];
  public fileToUpload: File | null = null;
  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;
  public isMenuVisible: boolean = false;

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
    public override readonly toastService: ToastService
  ) {
    super(toastService);
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
      this.eventService
        .add(
          {
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            gameId: parseInt(this.form.controls.gameId.value as any),
            startDate: this.form.controls.startDate.value,
            maxPeople: this.form.controls.maxPeople.value,
            isCustomImage: this.form.controls.isCustomImage.value,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.router.navigateByUrl('/events/home');
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
    if (this.form.valid && this.editEventId) {
      this.eventService
        .update(
          {
            id: this.editEventId,
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            gameId: parseInt(this.form.controls.gameId.value as any),
            startDate: this.form.controls.startDate.value,
            maxPeople: this.form.controls.maxPeople.value,
            isCustomImage: this.form.controls.isCustomImage.value,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.router.navigateByUrl('/events/home');
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

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'gameId':
        return 'Game';
      case 'name':
        return 'Name';
      case 'description':
        return 'Description';
      case 'startDate':
        return 'Start Date';
      case 'maxPeople':
        return 'Max People';
      case 'image':
        return 'Image';
      default:
        return controlName;
    }
  }

  private fetchEventById(id: number): void {
    this.eventService.getById(id).subscribe({
      next: (event) => {
        if (event) {
          this.form.patchValue({
            name: event.name,
            description: event.description,
            gameId: event.gameId,
            startDate: this.formatDate(event.startDate) as any,
            maxPeople: event.maxPeople,
            image: event.imageId.toString(),
            isCustomImage: event.isCustomImage,
          });

          this.eventService
            .getImage(event.isCustomImage, event.imageId)
            .subscribe((image) => (this.imagePreview = image));
          this.fileToUpload = null;

          this.initFormValueChanges();
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
  }

  private formatDate(date: string | Date): string {
    const d = new Date(date);
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    const year = d.getFullYear();
    return `${year}-${month}-${day}`;
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
      error: (error) => {
        console.log(error);
      },
    });
  }

  private fetchGameById(id: number): void {
    this.gameService.getById(id).subscribe({
      next: (game) => {
        if (game) {
          console.log(game);

          this.form.patchValue({
            image: game.imageId.toString(),
          });
          this.entityImagePipe
            .transform(ImageEntityType.Games, game.imageId)
            .subscribe((image) => {
              this.imagePreview = image;
              this.cd.detectChanges();
            });

          this.fileToUpload = null;
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

  private initFormValueChanges(): void {
    this.form.controls.gameId.valueChanges.subscribe((x) => {
      this.fetchGameById(x);
    });

    this.form.controls.isCustomImage.valueChanges.subscribe((x) => {
      if (x) {
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
      description: new FormControl<string | null>(null, Validators.required),
      startDate: new FormControl<Date | null>(null, [Validators.required]),
      maxPeople: new FormControl<number | null>(null, [
        Validators.required,
        Validators.min(1),
      ]),
      image: new FormControl<string | null>(null, [Validators.required]),
      isCustomImage: new FormControl<boolean>(false),
    });
  }
}
