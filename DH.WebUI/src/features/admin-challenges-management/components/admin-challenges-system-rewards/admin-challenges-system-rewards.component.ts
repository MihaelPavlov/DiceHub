import { Component } from '@angular/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Location } from '@angular/common';
import { RewardLevel } from '../../../../entities/rewards/enums/reward-level.enum';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { Router } from '@angular/router';
import { IRewardListResult } from '../../../../entities/rewards/models/reward-list.model';
import { throwError } from 'rxjs';
import { RewardImagePipe } from '../../../../shared/pipe/reward-image.pipe';
import { IRewardGetByIdResult } from '../../../../entities/rewards/models/reward-by-id.model';

interface ISystemRewardsForm {
  selectedLevel: number;
  requiredPoints: number;
  name: string;
  description: string;
  image: string;
}

interface ISystemRewardLevel {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-challenges-system-rewards',
  templateUrl: 'admin-challenges-system-rewards.component.html',
  styleUrl: 'admin-challenges-system-rewards.component.scss',
})
export class AdminChallengesSystemRewardsComponent extends Form {
  override form: Formify<ISystemRewardsForm>;

  public isMenuVisible: boolean = false;
  public imagePreview: string | ArrayBuffer | null = null;
  public fileToUpload: File | null = null;
  public imageError: string | null = null;
  public showRewardForm: boolean = false;
  public rewardLevels: ISystemRewardLevel[] = [];
  public rewardList: IRewardListResult[] = [];
  public editRewardId: number | null = null;

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly location: Location,
    private readonly rewardsService: RewardsService,
    private readonly rewardImagePipe: RewardImagePipe,
    private readonly router: Router
  ) {
    super(toastService);

    this.rewardLevels = Object.entries(RewardLevel)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({ id: value as number, name: key }));

    this.fetchSystemRewardList();

    this.form = this.initFormGroup();
  }

  public handleSearchExpression(searchExpression: string) {
    this.fetchSystemRewardList(searchExpression);
  }
  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public getFormGroup(formGroup: AbstractControl<any, any>): FormGroup {
    return formGroup as FormGroup;
  }

  public toggleRewardForm(isOpenFromEdit: boolean = false) {
    this.showRewardForm = !this.showRewardForm;

    if (!isOpenFromEdit) {
      this.form.reset();
      this.imagePreview = null;
      this.editRewardId = null;
    }
  }

  public onAddReward() {
    if (this.form.valid && this.fileToUpload) {
      this.rewardsService
        .add(
          {
            level: this.form.controls.selectedLevel.value,
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            requiredPoints: this.form.controls.requiredPoints.value,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.router.navigateByUrl('/admin-challenges/system-rewards');
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

  public onUpdateReward() {
    if (this.form.valid && this.editRewardId) {
      this.rewardsService
        .update(
          {
            id: this.editRewardId,
            level: this.form.controls.selectedLevel.value,
            name: this.form.controls.name.value,
            description: this.form.controls.description.value,
            requiredPoints: this.form.controls.requiredPoints.value,
            imageId: !this.fileToUpload
            ? +this.form.controls.image.value
            : null,
          },
          this.fileToUpload
        )
        .subscribe({
          next: (_) => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.fetchSystemRewardList();
            this.toggleRewardForm();
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

  public fillEditRewardForm(id: number) {
    this.editRewardId = id;
    this.rewardsService.getById(id).subscribe({
      next: (reward: IRewardGetByIdResult) => {
        this.form.patchValue({
          name: reward.name,
          description: reward.description,
          requiredPoints: reward.requiredPoints,
          selectedLevel: reward.level,
          image: reward.imageId.toString(),
        });
        this.imagePreview = this.rewardImagePipe.transform(reward.imageId);
        this.fileToUpload = null;
        this.toggleRewardForm(true);
      },
      error: (error) => {
        this.editRewardId = null;
        throwError(() => error);
      },
    });
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
      this.imageError = 'Image is required.';
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
      case 'selectedLevel':
        return 'Level';
      case 'name':
        return 'Name';
      case 'description':
        return 'Description';
      case 'requiredPoints':
        return 'Required Points';
      case 'image':
        return 'Image';
      default:
        return controlName;
    }
  }

  private fetchSystemRewardList(searchExpression: string = '') {
    this.rewardsService.getList(searchExpression).subscribe({
      next: (rewardList) => {
        this.rewardList = rewardList ?? [];
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      selectedLevel: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      requiredPoints: new FormControl<number>(0, [Validators.required]),
      name: new FormControl<string>('', [Validators.required]),

      description: new FormControl<string>('', [Validators.required]),
      image: new FormControl<string | null>('', [Validators.required]),
    });
  }
}