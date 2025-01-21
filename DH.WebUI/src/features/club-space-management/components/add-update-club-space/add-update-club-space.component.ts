import { SpaceManagementService } from './../../../../entities/space-management/api/space-management.service';
import { Component, OnInit } from '@angular/core';
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
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { MatDialog } from '@angular/material/dialog';
import { SinglePlayerConfirmDialog } from '../../dialogs/single-player-confirm-dialog/single-player-confirm-dialog.component';
import { Location } from '@angular/common';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../shared/pipe/entity-image.pipe';

interface ICreateSpaceTableForm {
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
export class AddUpdateClubSpaceComponent extends Form implements OnInit {
  override form: Formify<ICreateSpaceTableForm>;

  public imagePreview: string | ArrayBuffer | SafeUrl | null = null;
  public editTableId: number | null = null;

  public showPassword = false;
  private gameId: number | null = null;

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly activatedRoute: ActivatedRoute,
    private readonly gamesService: GamesService,
    private readonly spaceManagementService: SpaceManagementService,
    private readonly entityImagePipe: EntityImagePipe,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly location: Location
  ) {
    super(toastService);
    this.form = this.initFormGroup();
  }

  public ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params) => {
      this.gameId = Number.parseInt(params.get('gameId') ?? '');
      const tableId = params.get('tableId');

      if (this.gameId) {
        this.fetchGameByIdForCreate(this.gameId);
      } else if (tableId) {
        this.editTableId = Number.parseInt(tableId);
        this.spaceManagementService.getTableById(this.editTableId).subscribe({
          next: (result) => {
            this.form.patchValue({
              tableName: result.name,
              maxPeople: result.maxPeople,
              password: result.password,
            });
            this.fetchGameByIdForUpdate(result.gameId);
          },
        });
      }
    });
  }

  public get getHeader(): string {
    return this.editTableId ? 'Update Game Table' : 'Create Game Table ';
  }

  public showInfoForGame(): void {
    this.router.navigateByUrl(`games/${this.gameId}/details`);
  }

  public backNavigateBtn(): void {
    if (this.editTableId)
      this.router.navigateByUrl(`space/${this.editTableId}/details`);

    this.location.back();
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
          isSoloModeActive: false,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });
            this.router.navigateByUrl('/space/home');
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

  public onUpdate() {
    if (this.form.valid && this.editTableId) {
      this.spaceManagementService
        .update({
          id: this.editTableId,
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
            this.router.navigateByUrl(`/space/${this.editTableId}/details`);
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

  private fetchGameByIdForUpdate(gameId: number): void {
    this.gamesService.getById(gameId).subscribe({
      next: (game: IGameByIdResult) => {
        if (game) {
          this.gameId = game.id;
          this.form.patchValue({
            gameName: game.name,
            gameId: game.id,
          });

          this.entityImagePipe
            .transform(ImageEntityType.Games, game.imageId)
            .subscribe((x) => (this.imagePreview = x));
        }
      },
    });
  }

  private fetchGameByIdForCreate(gameId: number): void {
    this.gamesService.getById(gameId).subscribe({
      next: (game: IGameByIdResult) => {
        if (game) {
          this.form.patchValue({
            gameName: game.name,
            gameId: game.id,
          });

          this.entityImagePipe
            .transform(ImageEntityType.Games, game.imageId)
            .subscribe((x) => (this.imagePreview = x));

          if (game.minPlayers === 1) {
            const dialogRef = this.dialog.open(SinglePlayerConfirmDialog, {});

            dialogRef.afterClosed().subscribe((result) => {
              if (result) {
                this.spaceManagementService
                  .add({
                    gameId: this.form.controls.gameId.value,
                    name: '',
                    maxPeople: 0,
                    password: '',
                    isSoloModeActive: true,
                  })
                  .subscribe({
                    next: () => {
                      this.toastService.success({
                        message: AppToastMessage.ChangesSaved,
                        type: ToastType.Success,
                      });
                      this.router.navigateByUrl('/space/home');
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
            });
          }
        }
      },
      error: (error) => {
        throwError(() => error);
      },
    });
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
