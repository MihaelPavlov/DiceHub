import { SpaceManagementService } from './../../../../entities/space-management/api/space-management.service';
import { Component } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ToastService } from '../../../../shared/services/toast.service';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GamesService } from '../../../../entities/games/api/games.service';
import { throwError } from 'rxjs';
import { IGameByIdResult } from '../../../../entities/games/models/game-by-id.model';
import { SafeUrl } from '@angular/platform-browser';
import { GameImagePipe } from '../../../../shared/pipe/game-image.pipe';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

interface ICreateSpaceTable {
  gameName: string;
  gameId: number;
  tableName: string;
  maxPeople: number;
  password: string;
}

@Component({
  selector: 'app-add-update-club-space',
  templateUrl: 'add-update-club-space.component.html',
  styleUrl: 'add-update-club-space.component.scss',
})
export class AddUpdateClubSpaceComponent extends Form {
  override form: Formify<ICreateSpaceTable>;
  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;

  public showPassword = false;
  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly activatedRoute: ActivatedRoute,
    private readonly gamesService: GamesService,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly gameImagePipe: GameImagePipe,
    private readonly router: Router
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.activatedRoute.paramMap.subscribe((params) => {
      const id = params.get('gameId');
      if (id) {
        this.gamesService.getById(Number.parseInt(id)).subscribe({
          next: (game: IGameByIdResult) => {
            if (game) {
              this.form.patchValue({
                gameName: game.name,
                gameId: game.id,
              });

              this.imagePreview = this.gameImagePipe.transform(game.imageId);
            }
          },
          error: (error) => {
            throwError(() => error);
          },
        });
      }
    });
  }
  public togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  public onAdd() {
    if (this.form.valid) {
      this.spaceManagementService
        .add({
          gameId: this.form.controls.gameId.value,
          name: this.form.controls.tableName.value,
          maxPeople: this.form.controls.maxPeople.value,
          password: this.form.controls.password.value,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });
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

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'gameName':
        return 'Scanned Game';
      case 'tableName':
        return 'Table Name';
      case 'maxPeople':
        return 'Max People';
      case 'password':
        return 'Password';
      default:
        return controlName;
    }
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      gameName: new FormControl<string>('', [Validators.required]),
      gameId: new FormControl<number | null>(null),
      tableName: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      maxPeople: new FormControl<number>(3, [Validators.required]),
      password: new FormControl<string>(''),
    });
  }
}
