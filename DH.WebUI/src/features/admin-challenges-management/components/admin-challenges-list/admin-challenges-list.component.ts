import { IChallengeResult } from './../../../../entities/challenges/models/challenge-by-id.model';
import { AdminChallengesConfirmDeleteDialog } from './../../dialogs/admin-challenges-confirm-delete/admin-challenges-confirm-delete.component';
import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { IGameDropdownResult } from '../../../../entities/games/models/game-dropdown.model';
import { IGameListResult } from '../../../../entities/games/models/game-list.model';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { GamesService } from '../../../../entities/games/api/games.service';
import { Form } from '../../../../shared/components/form/form.component';
import { ToastService } from '../../../../shared/services/toast.service';
import { Location } from '@angular/common';
import { Formify } from '../../../../shared/models/form.model';
import { ChallengesService } from '../../../../entities/challenges/api/challenges.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { IChallengeListResult } from '../../../../entities/challenges/models/challenge-list.model';
import { ChallengeType } from '../../../../entities/challenges/enums/challenge-type.enum';
import { MatDialog } from '@angular/material/dialog';
import { throwError } from 'rxjs';
import { ChallengeRewardPoint } from '../../../../entities/challenges/enums/challenge-reward-point.enum';
import { ScrollService } from '../../../../shared/services/scroll.service';
interface IChallengeForm {
  selectedGame: number;
  rewardPoints: number;
  attempts: number;
  type: number;
  description: string;
}

interface IDropdown {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-challenges-list',
  templateUrl: 'admin-challenges-list.component.html',
  styleUrl: 'admin-challenges-list.component.scss',
})
export class AdminChallengesListComponent extends Form {
  override form: Formify<IChallengeForm>;

  public gameList: IGameDropdownResult[] = [];
  public games: IGameListResult[] = [];
  public showChallengeForm: boolean = false;
  public showFilter: boolean = false;
  public editChallengeId: number | null = null;
  public challengeList: IChallengeListResult[] = [];
  public filterGameIds: IGameDropdownResult[] = [];
  public challengeTypeList: IDropdown[] = [];
  public challengeRewardPointList: IDropdown[] = [];
  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly challengesService: ChallengesService,
    private readonly fb: FormBuilder,
    private readonly location: Location,
    private readonly dialog: MatDialog,
    private readonly scrollService: ScrollService
  ) {
    super(toastService);

    this.fetchGameList();
    this.fetchChallengeList();

    this.challengeTypeList = Object.entries(ChallengeType)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.challengeRewardPointList = Object.entries(ChallengeRewardPoint)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: value.toString() }));

    this.gameService.getList('').subscribe({
      next: (gameList) => (this.games = gameList ?? []),
      error: (error) => {
        console.log(error);
      },
    });
    this.form = this.initFormGroup();
  }

  public openDeleteDialog(id: number): void {
    const dialogRef = this.dialog.open(AdminChallengesConfirmDeleteDialog, {
      width: '17rem',
      position: { bottom: '80%', left: '2%' },
      data: { id: id },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchChallengeList();
        if (this.showChallengeForm) this.toggleChallengeForm();
      }
    });
  }

  public toggleFilter(): void {
    this.showFilter = !this.showFilter;
  }

  public toggleChallengeForm(isOpenFromEdit: boolean = false): void {
    this.showChallengeForm = !this.showChallengeForm;

    if (!isOpenFromEdit) {
      this.form.reset();
      this.editChallengeId = null;
    }
  }

  public backNavigateBtn() {
    this.location.back();
  }

  public fillEditChallengeForm(id: number): void {
    this.editChallengeId = id;
    this.challengesService.getById(id).subscribe({
      next: (challenge: IChallengeResult) => {
        this.form.patchValue({
          description: challenge.description,
          selectedGame: challenge.gameId,
          attempts: challenge.attempts,
          rewardPoints: challenge.rewardPoints,
          type: challenge.type,
        });
        this.showChallengeForm = true;
        this.scrollService.scrollToTop();
      },
      error: (error) => {
        this.editChallengeId = null;
        throwError(() => error);
      },
    });
  }

  public onAddChallenge(): void {
    if (this.form.valid) {
      this.challengesService
        .add({
          gameId: this.form.controls.selectedGame.value,
          rewardPoints: this.form.controls.rewardPoints.value,
          description: this.form.controls.description.value,
          attempts: this.form.controls.attempts.value,
          type: this.form.controls.type.value,
        })
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.fetchChallengeList();
            this.toggleChallengeForm();
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

  public onUpdateChallenge(): void {
    if (this.form.valid && this.editChallengeId) {
      this.challengesService
        .update({
          id: this.editChallengeId,
          gameId: this.form.controls.selectedGame.value,
          rewardPoints: this.form.controls.rewardPoints.value,
          description: this.form.controls.description.value,
          attempts: this.form.controls.attempts.value,
          type: this.form.controls.type.value,
        })
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.fetchChallengeList();
            this.toggleChallengeForm();
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

  public fetchChallengeList(): void {
    this.challengesService
      .getList(this.filterGameIds.map((x) => x.id))
      .subscribe({
        next: (challengeList) => {
          this.challengeList = challengeList ?? [];
        },
        error: (error) => {
          console.log(error);
        },
      });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'selectedGame':
        return 'Game';
      case 'description':
        return 'Description';
      case 'attempts':
        return 'Attempts';
      case 'rewardPoints':
        return 'Reward Points';
      case 'type':
        return 'Type';
      default:
        return controlName;
    }
  }

  private fetchGameList(): void {
    this.gameService.getDropdownList().subscribe({
      next: (gameList) => {
        this.gameList = gameList ?? [];
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      selectedGame: new FormControl<number | null>(null, [Validators.required]),
      rewardPoints: new FormControl<number>(0, [Validators.required]),
      attempts: new FormControl<number>(0, [Validators.required]),
      type: new FormControl<number | null>(null, [Validators.required]),
      description: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(50),
      ]),
    });
  }
}
