import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
  Pipe,
  PipeTransform,
} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { GamesService } from '../../../../../entities/games/api/games.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MenuTabsService } from '../../../../../shared/services/menu-tabs.service';
import { Router } from '@angular/router';
import { NAV_ITEM_LABELS } from '../../../../../shared/models/nav-items-labels.const';
import { GameCategoriesService } from '../../../../../entities/games/api/game-categories.service';
import { IGameCategory } from '../../../../../entities/games/models/game-category.model';
import { Observable } from 'rxjs';
import { ToastType } from '../../../../../shared/models/toast.model';
import { Form } from '../../../../../shared/components/form/form.component';

interface ICreateForm {
  categoryId: number;
  name: string;
  description: string;
  minAge: number;
  minPlayers: number;
  maxPlayers: number;
  averagePlaytime: number;
  image: string;
}
export type Formify<T> = FormGroup<{
  [K in keyof T]: FormControl<T[K]>;
}>;

@Component({
  selector: 'app-add-update-game',
  templateUrl: 'add-update-game.component.html',
  styleUrl: 'add-update-game.component.scss',
})
export class AddUpdateGameComponent extends Form implements OnInit {
  override form: Formify<ICreateForm>;
  public categories!: Observable<IGameCategory[] | null>;
  public imagePreview: string | ArrayBuffer | null = null;
  public isEdit: boolean = false;
  public showQRCode = false;

  public games;
  selectedGame: any;

  public isMenuVisible: boolean = false;
  public imageError: string | null = null;
  public fileToUpload: File | null = null;
  constructor(
    private readonly fb: FormBuilder,
    private readonly gameService: GamesService,
    private readonly gameCategoriesService: GameCategoriesService,
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly router: Router,
    private readonly cd: ChangeDetectorRef
  ) {
    super(toastService);
    this.form = this.initFormGroup()
    this.menuTabsService.setActive(NAV_ITEM_LABELS.GAMES);
  }
  private initFormGroup(): FormGroup {
    return this.fb.group({
      categoryId: new FormControl<null | number>(null, [Validators.required]),
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
      averagePlaytime: new FormControl<number>(0, [Validators.required]),
      image: new FormControl<string | null>(null, [Validators.required]),
    });
  }
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public ngOnInit(): void {
    // this.form = this.fb.nonNullable.group({
    //   categoryId: ['', Validators.required],
    //   name: ['', [Validators.required, Validators.minLength(3)]],
    //   description: ['', Validators.required],
    //   minAge: [12, Validators.required],
    //   minPlayers: [null, [Validators.required, Validators.min(1)]],
    //   maxPlayers: [null, [Validators.required, Validators.min(1)]],
    //   averagePlaytime: [0, Validators.required],
    //   image: [null, Validators.required],
    // });

    this.categories = this.gameCategoriesService.getList();
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('games/library');
  }

  public onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    console.log((event.target as HTMLInputElement).files);

    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
        this.fileToUpload = file;
        this.imageError = null;
        this.form.controls['image'].patchValue(file.name);
      };
      reader.readAsDataURL(file);
    } else {
      this.imageError = 'Image is required.';
      this.fileToUpload = null;
      this.imagePreview = null;
      this.form.controls['image'].reset();
    }
  }

  public firstErrorMessage(): string | null {
    const controls = this.form.controls;
    for (const controlName in controls) {
      if (controls.hasOwnProperty(controlName)) {
        const errorMessage = this.getErrorMessage(controlName);
        if (errorMessage) {
          return errorMessage;
        }
      }
    }

    return this.imageError ? this.imageError : null;
  }

  //TODO: move it in form component
  public getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);

    if (control && control.errors && (control.touched || control.dirty)) {
      if (control.errors['required']) {
        return `${this.getControlDisplayName(controlName)} is required.`;
      } else if (control.errors['minlength']) {
        const requiredLength = control.errors['minlength'].requiredLength;
        return `${this.getControlDisplayName(
          controlName
        )} must be at least ${requiredLength} characters long.`;
      } else if (control.errors['maxlength']) {
        const requiredLength = control.errors['maxlength'].requiredLength;
        return `${this.getControlDisplayName(
          controlName
        )} cannot exceed ${requiredLength} characters.`;
      } else if (control.errors['min']) {
        return `${this.getControlDisplayName(controlName)} must be at least ${
          control.errors['min'].min
        }.`;
      }

      if (this.getFirstErrorMessage) {
        return this.getFirstErrorMessage;
      }
    }

    return '';
  }

  public getControlDisplayName(controlName: string): string {
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

  public onSubmit(): void {
    if (!this.fileToUpload) {
      this.imageError = 'Image is required.';

      return;
    }

    if (this.form.valid && this.fileToUpload) {
      this.gameService
        .add(
          {
            categoryId:
              parseInt(this.form.controls.categoryId.value as any)
            ,
            name: this.form.controls.name.value,
            description: this.form.controls.name.value,
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
              message: 'Game Successfully Created',
              type: ToastType.Success,
            });

            //TODO: Just an idea maybe show qr after redirect from creating ?
            this.router.navigateByUrl('/games/library');
          },
          error: (error) => {
            this.handleErrors(error);
            // this.toastService.error({
            //   message:
            //     'An error occurred while creating the game. Please try again.',
            //   type: ToastType.Error,
            // });
            // Optionally, log the error or perform other actions here
            console.error('Error creating game:', error);
          },
        });
    }
  }
}
