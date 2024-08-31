import { Component, OnDestroy, OnInit } from '@angular/core';
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
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { IGameDropdownResult } from '../../../../../entities/games/models/game-dropdown.model';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { throwError } from 'rxjs';
import { EventsService } from '../../../../../entities/events/api/events.service';
import { AppToastMessage } from '../../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../../shared/models/toast.model';

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
  public imagePreview: string | ArrayBuffer | null = null;
  public isMenuVisible: boolean = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly gameService: GamesService,
    private readonly eventService: EventsService,
    public override readonly toastService: ToastService
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe((x) => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
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
    this.menuTabsService.setActive(NAV_ITEM_LABELS.EVENTS);
  }
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }
  public ngOnInit(): void {
    this.fetchGameList();
  }

  public ngOnDestroy(): void {}

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
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

            this.router.navigateByUrl('/admin-events');
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

  public onUpdate(): void {}

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
    this.router.navigateByUrl('admin-events');
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
          this.imagePreview = `https://localhost:7024/games/get-image/${game.imageId}`;
          this.fileToUpload = null;
        }
      },
      error: (error) => {
        throwError(() => error);
      },
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
