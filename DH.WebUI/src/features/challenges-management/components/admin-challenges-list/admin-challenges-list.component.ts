import { LanguageService } from './../../../../shared/services/language.service';
import { ChallengeType } from './../../../../pages/challenges-management/shared/challenge-type.enum';
import { IChallengeResult } from '../../../../entities/challenges/models/challenge-by-id.model';
import { AdminChallengesConfirmDeleteDialog } from '../../dialogs/admin-challenges-confirm-delete/admin-challenges-confirm-delete.component';
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
import { MatDialog } from '@angular/material/dialog';
import { throwError } from 'rxjs';
import { ChallengeRewardPoint } from '../../../../entities/challenges/enums/challenge-reward-point.enum';
import { ScrollService } from '../../../../shared/services/scroll.service';
import { ImageEntityType } from '../../../../shared/pipe/entity-image.pipe';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { TranslateService } from '@ngx-translate/core';
import { UniversalChallengeType } from '../../../../pages/challenges-management/shared/challenge-universal-type.enum';
import { IUniversalChallengeListResult } from '../../../../entities/challenges/models/universal-challenge.model';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';

interface IChallengeForm {
  selectedGame: number;
  rewardPoints: number;
  attempts: number;
  type: number;
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
  public universalChallengeList: IUniversalChallengeListResult[] = [];
  public filterGameIds: IGameDropdownResult[] = [];
  public challengeRewardPointList: IDropdown[] = [];
  public readonly ImageEntityType = ImageEntityType;
  public readonly ChallengeType = ChallengeType;

  constructor(
    public override readonly toastService: ToastService,
    private readonly gameService: GamesService,
    private readonly challengesService: ChallengesService,
    private readonly fb: FormBuilder,
    private readonly location: Location,
    private readonly dialog: MatDialog,
    private readonly scrollService: ScrollService,
    public override translateService: TranslateService,
    public readonly languageService: LanguageService
  ) {
    super(toastService, translateService);
    this.fetchGameList();
    this.fetchChallengeList();
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
          selectedGame: challenge.gameId,
          attempts: challenge.attempts,
          rewardPoints: challenge.rewardPoints,
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
          attempts: this.form.controls.attempts.value,
        })
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });

            this.fetchChallengeList();
            this.toggleChallengeForm();
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

  public onUpdateChallenge(): void {
    if (this.form.valid && this.editChallengeId) {
      this.challengesService
        .update({
          id: this.editChallengeId,
          gameId: this.form.controls.selectedGame.value,
          rewardPoints: this.form.controls.rewardPoints.value,
          attempts: this.form.controls.attempts.value,
        })
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });

            this.fetchChallengeList();
            this.toggleChallengeForm();
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
        return this.translateService.instant(
          'admin_challenge.control_display_names.game'
        );
      case 'attempts':
        return this.translateService.instant(
          'admin_challenge.control_display_names.attempts'
        );
      case 'rewardPoints':
        return this.translateService.instant(
          'admin_challenge.control_display_names.points'
        );
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
    });
  }
}
