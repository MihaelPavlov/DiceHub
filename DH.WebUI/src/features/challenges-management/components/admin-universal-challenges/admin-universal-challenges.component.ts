import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ChallengesService } from '../../../../entities/challenges/api/challenges.service';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { ChallengeRewardPoint } from '../../../../entities/challenges/enums/challenge-reward-point.enum';
import { IUniversalChallengeListResult } from '../../../../entities/challenges/models/universal-challenge.model';
import { LanguageService } from '../../../../shared/services/language.service';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { IUpdateUniversalChallengeDto } from '../../../../entities/challenges/models/update-universal-challenge.model';

@Component({
    selector: 'app-admin-universal-challenges',
    templateUrl: './admin-universal-challenges.component.html',
    styleUrls: ['./admin-universal-challenges.component.scss'],
    standalone: false
})
export class AdminUniversalChallengesComponent implements OnInit {
  public universalChallengeList: IUniversalChallengeListResult[] = [];
  public challengeRewardPointList: IDropdown[] = [];
  public SupportLanguages = SupportLanguages;

  public form: FormArray = this.fb.array([]);

  constructor(
    private readonly fb: FormBuilder,
    private readonly challengesService: ChallengesService,
    private readonly languageService: LanguageService,
    private readonly translateService: TranslateService,
    private readonly toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.challengeRewardPointList = Object.entries(ChallengeRewardPoint)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: value.toString() }));

    this.fetchUniversalChallengeList();
  }

  public getChallengeFormGroup(i: number): FormGroup {
    return this.form.at(i) as FormGroup;
  }

  public getCurrentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public fetchUniversalChallengeList(): void {
    this.challengesService.getUniversalList().subscribe({
      next: (result) => {
        this.universalChallengeList = result ?? [];
        this.initForms();
      },
    });
  }

  public saveChallenge(index: number): void {
    const challengeForm = this.form.at(index) as FormGroup;
    if (challengeForm.valid) {
      const updatedChallenge =
        challengeForm.value as IUpdateUniversalChallengeDto;
      this.challengesService
        .updateUniversalChallenge(updatedChallenge)
        .subscribe({
          next: () => {
            this.toastService.success({
              message: this.translateService.instant(
                AppToastMessage.ChangesSaved
              ),
              type: ToastType.Success,
            });
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
  }

  private initForms(): void {
    this.form.clear();
    this.universalChallengeList.forEach((challenge) => {
      const fg = this.fb.group({
        id: [challenge.id],
        attempts: [challenge.attempts ?? 0, Validators.required],
        rewardPoints: [challenge.rewardPoints ?? 0, Validators.required],
        minValue: [challenge.minValue ?? null],
      });
      this.form.push(fg);
    });
  }
}
