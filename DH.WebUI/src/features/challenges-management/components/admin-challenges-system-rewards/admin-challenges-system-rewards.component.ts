import { ScrollService } from '../../../../shared/services/scroll.service';
import { ChangeDetectorRef, Component, NgZone, OnDestroy } from '@angular/core';
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
import { RewardLevel } from '../../../../entities/rewards/enums/reward-level.enum';
import { RewardsService } from '../../../../entities/rewards/api/rewards.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';
import { IRewardListResult } from '../../../../entities/rewards/models/reward-list.model';
import { debounceTime, distinctUntilChanged, Subscription, throwError } from 'rxjs';
import {
  FormDraftService,
  IFormDraftOptions,
} from '../../../../shared/services/form-draft.service';
import { IRewardGetByIdResult } from '../../../../entities/rewards/models/reward-by-id.model';
import { AdminChallengesRewardConfirmDeleteDialog } from '../../dialogs/admin-challenges-reward-confirm-delete/admin-challenges-reward-confirm-delete.component';
import { MatDialog } from '@angular/material/dialog';
import { REWARD_POINTS } from '../../../../entities/rewards/enums/reward-required-point.enum';
import {
  EntityImagePipe,
  ImageEntityType,
} from '../../../../shared/pipe/entity-image.pipe';
import { IDropdown } from '../../../../shared/models/dropdown.model';
import { TranslateService } from '@ngx-translate/core';
import { SupportLanguages } from '../../../../entities/common/models/support-languages.enum';
import { LanguageService } from '../../../../shared/services/language.service';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Capacitor } from '@capacitor/core';
import { downscaleImageFile } from '../../../../shared/helpers/image-resize.helper';
import { FrontEndLogService } from '../../../../shared/services/frontend-log.service';

interface ISystemRewardsForm {
  selectedLevel: number;
  requiredPoints: number;
  name_en: string;
  name_bg: string;
  description_en: string;
  description_bg: string;
  cashEquivalent: number;
  image: string;
}

@Component({
    selector: 'app-admin-challenges-system-rewards',
    templateUrl: 'admin-challenges-system-rewards.component.html',
    styleUrl: 'admin-challenges-system-rewards.component.scss',
    standalone: false
})
export class AdminChallengesSystemRewardsComponent extends Form implements OnDestroy {
  override form: Formify<ISystemRewardsForm>;
  private static readonly DraftKey = 'adminChallengesSystemRewards';
  // 'image' holds only a filename string; the actual File can't be JSON-persisted.
  private static readonly DraftOptions: IFormDraftOptions = { exclude: ['image'] };
  private draftSubscription: Subscription | null = null;

  public isMenuVisible: boolean = false;
  public imagePreview: string | ArrayBuffer | null = null;
  public fileToUpload: File | null = null;
  public imageError: string | null = null;
  public showRewardForm: boolean = false;
  public rewardLevels: IDropdown[] = [];
  public rewardRequiredPointList: IDropdown[] = [];
  public rewardList: IRewardListResult[] = [];
  public editRewardId: number | null = null;
  public readonly ImageEntityType = ImageEntityType;
  private skipFirstSelectedLevelChange = true;
  public showSearch: boolean = false;
  public searchForm!: FormGroup;
  public currentLangDescription: 'EN' | 'BG' = 'EN';
  public currentLangName: 'EN' | 'BG' = 'EN';
  public readonly SupportLanguages = SupportLanguages;

  constructor(
    public override readonly toastService: ToastService,
    private readonly fb: FormBuilder,
    private readonly rewardsService: RewardsService,
    private readonly entityImagePipe: EntityImagePipe,
    private readonly dialog: MatDialog,
    private readonly scrollService: ScrollService,
    public override translateService: TranslateService,
    private readonly languageService: LanguageService,
    private readonly formDraftService: FormDraftService,
    private readonly frontEndLogService: FrontEndLogService,
    private readonly ngZone: NgZone,
    private readonly cd: ChangeDetectorRef
  ) {
    super(toastService, translateService);

    this.rewardLevels = Object.entries(RewardLevel)
      .filter(([key, value]) => typeof value === 'number')
      .map(([key, value]) => ({
        id: value as number,
        name: this.translateService.instant('reward_level.' + key),
      }));

    this.fetchSystemRewardList();

    this.form = this.initFormGroup();

    this.form.controls.selectedLevel.valueChanges.subscribe((selectedLevel) => {
      this.updateRequiredPoints(selectedLevel);
    });

    // Wired up after the selectedLevel subscription above, not before: restoring a
    // draft with a selectedLevel patches that control's value, which needs the
    // subscription already active to correctly re-enable/populate requiredPoints.
    // 'image' holds a filename string, but the actual File can't be persisted -
    // Android can kill the app process while the native image picker is open,
    // which wipes the pending File selection with no way to recover it. Surface
    // that gap instead of leaving the user staring at a silently-empty form.
    // hasDraft() only reports true for a recent draft that still holds real
    // user input, so a bare form.reset() re-persisting an empty form no longer
    // produces a bogus "draft restored, image lost" error on the next visit.
    const hadDraft = this.formDraftService.hasDraft(
      AdminChallengesSystemRewardsComponent.DraftKey,
      AdminChallengesSystemRewardsComponent.DraftOptions
    );
    this.startDraftAutoSave();
    if (hadDraft) {
      this.showRewardForm = true;
      this.toastService.error({
        message: this.translateService.instant(
          'admin_rewards.draft_restored_reselect_image'
        ),
        type: ToastType.Error,
      });
    }

    this.searchForm = this.fb.group({
      search: [''],
    });

    this.searchForm
      .get('search')!
      .valueChanges.pipe(debounceTime(1000), distinctUntilChanged())
      .subscribe((searchExpression: string) => {
        this.onSearchSubmit(searchExpression);
      });
  }

  public get currentLanguage(): SupportLanguages {
    return this.languageService.getCurrentLanguage();
  }

  public setLangDescription(lang: 'EN' | 'BG') {
    this.currentLangDescription = lang;
  }

  public setLangName(lang: 'EN' | 'BG') {
    this.currentLangName = lang;
  }

  public toggleSearch(): void {
    this.showSearch = !this.showSearch;
  }

  private onSearchSubmit(searchExpression: string) {
    this.fetchSystemRewardList(searchExpression);
  }

  public updateRequiredPoints(selectedLevel: number) {
    if (Object.values(RewardLevel).includes(selectedLevel)) {
      this.form.controls.requiredPoints.enable();
      this.rewardRequiredPointList = REWARD_POINTS[selectedLevel] || [];
    } else {
      this.form.controls.requiredPoints.disable();
      this.rewardRequiredPointList = [];
    }
    if (this.skipFirstSelectedLevelChange) {
      this.skipFirstSelectedLevelChange = false;
      return;
    } else {
      this.form.controls.requiredPoints.reset();
    }
  }

  public openDeleteDialog(id: number): void {
    const dialogRef = this.dialog.open(
      AdminChallengesRewardConfirmDeleteDialog,
      {
        data: { id: id },
      }
    );

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.fetchSystemRewardList();
        if (this.showRewardForm) this.toggleRewardForm();
      }
    });
  }

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public ngOnDestroy(): void {
    this.draftSubscription?.unsubscribe();
  }

  private startDraftAutoSave(): void {
    if (this.draftSubscription) return;
    this.draftSubscription = this.formDraftService.autoSave(
      this.form,
      AdminChallengesSystemRewardsComponent.DraftKey,
      AdminChallengesSystemRewardsComponent.DraftOptions
    );
  }

  /**
   * Tears down the debounced auto-save, including any write still pending in its
   * debounce window. Needed before we clear + reset the form on close/submit:
   * otherwise the programmatic form.reset() re-persists an empty draft moments
   * after clear(), which then trips the "draft restored, image lost" error on
   * the next visit even though the user typed nothing.
   */
  private stopDraftAutoSave(): void {
    this.draftSubscription?.unsubscribe();
    this.draftSubscription = null;
  }

  private discardDraftAndReset(): void {
    this.stopDraftAutoSave();
    this.formDraftService.clear(AdminChallengesSystemRewardsComponent.DraftKey);
    this.resetRewardForm();
    this.startDraftAutoSave();
  }

  public getFormGroup(formGroup: AbstractControl<any, any>): FormGroup {
    return formGroup as FormGroup;
  }

  public openRewardForm(): void {
    this.showRewardForm = true;
  }

  public closeRewardForm(event?: Event): void {
    event?.stopPropagation();
    this.showRewardForm = false;
    this.discardDraftAndReset();
  }

  public toggleRewardForm(isOpenFromEdit: boolean = false): void {
    if (this.showRewardForm && !isOpenFromEdit) {
      this.closeRewardForm();
      return;
    }

    this.showRewardForm = true;
  }

  public onAddReward() {
    if (this.form.valid && this.fileToUpload) {
      this.rewardsService
        .add(
          {
            level: this.form.controls.selectedLevel.value,
            name_EN: this.form.controls.name_en.value,
            name_BG: this.form.controls.name_bg.value,
            cashEquivalent: this.form.controls.cashEquivalent.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
            requiredPoints: this.form.controls.requiredPoints.value,
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

            this.fetchSystemRewardList();
            // Closes the panel, which clears the draft and resets the form.
            this.toggleRewardForm();
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

  public onUpdateReward() {
    if (this.form.valid && this.editRewardId) {
      this.rewardsService
        .update(
          {
            id: this.editRewardId,
            level: this.form.controls.selectedLevel.value,
            name_EN: this.form.controls.name_en.value,
            name_BG: this.form.controls.name_bg.value,
            description_EN: this.form.controls.description_en.value,
            description_BG: this.form.controls.description_bg.value,
            cashEquivalent: this.form.controls.cashEquivalent.value,
            requiredPoints: this.form.controls.requiredPoints.value,
            imageUrl: !this.fileToUpload
              ? this.form.controls.image.value
              : null,
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

            this.fetchSystemRewardList();
            // Closes the panel, which clears the draft and resets the form.
            this.toggleRewardForm();
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

  public fillEditRewardForm(id: number) {
    this.editRewardId = id;
    this.skipFirstSelectedLevelChange = true;
    this.rewardsService.getById(id).subscribe({
      next: (reward: IRewardGetByIdResult) => {
        this.form.patchValue({
          name_en: reward.name_EN,
          name_bg: reward.name_BG,
          description_en: reward.description_EN,
          description_bg: reward.description_BG,
          requiredPoints: reward.requiredPoints,
          cashEquivalent: reward.cashEquivalent,
          selectedLevel: reward.level,
          image: reward.imageUrl,
        });
        this.imagePreview = reward.imageUrl;
        this.fileToUpload = null;
        this.showRewardForm = true;
        this.scrollService.scrollToTop();
      },
      error: (error) => {
        this.editRewardId = null;
        throwError(() => error);
      },
    });
  }

  /**
   * Uses Capacitor's native Camera plugin instead of a raw <input type="file">.
   * On Android, opening the WebView's own file-chooser for that raw input has
   * proven to reliably get the app's process killed while the native picker is
   * in the foreground, losing the selection entirely. The Camera plugin drives
   * the OS picker through Android's native Activity-result flow instead, which
   * doesn't have that failure mode. Works identically on web (Capacitor falls
   * back to a file input internally there).
   */
  public async pickRewardImage(): Promise<void> {
    try {
      const photo = await Camera.getPhoto({
        source: CameraSource.Photos,
        resultType: CameraResultType.Uri,
        quality: 70,
        width: 1280,
      });

      if (!photo.webPath) {
        this.reportImagePickFailure('Camera.getPhoto returned no webPath', '');
        return;
      }

      const webPath = photo.webPath;
      const blob = await (await fetch(webPath)).blob();
      const extension = photo.format || 'jpeg';
      let file = new File([blob], `reward-image.${extension}`, {
        type: blob.type || `image/${extension}`,
      });

      // On web the plugin's width/quality hints don't apply - shrink here so a
      // multi-MB photo isn't relayed through the API. On native the plugin
      // already downscaled, so skip the extra re-encode.
      if (Capacitor.getPlatform() === 'web') {
        file = await downscaleImageFile(file);
      }

      // Capacitor plugin callbacks aren't always guaranteed to run inside
      // Angular's zone - force it explicitly so the preview/form actually
      // re-render instead of silently updating state nobody sees.
      this.ngZone.run(() => {
        this.imagePreview = webPath;
        this.form.controls.image.patchValue(file.name);
        this.fileToUpload = file;
        this.imageError = null;
        this.cd.markForCheck();
      });
    } catch (error: any) {
      if (error?.message === 'User cancelled photos app') {
        return;
      }
      this.reportImagePickFailure(error?.message ?? String(error), error?.stack ?? '');
    }
  }

  private reportImagePickFailure(message: string, stack: string): void {
    this.frontEndLogService
      .sendWarning(`Reward image pick failed: ${message}`, stack)
      .subscribe();
    this.ngZone.run(() => {
      this.toastService.error({
        message: this.translateService.instant(AppToastMessage.SomethingWrong),
        type: ToastType.Error,
      });
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'selectedLevel':
        return this.translateService.instant(
          'admin_rewards.control_display_names.selected_level'
        );
      case 'name_en':
        return this.translateService.instant(
          'admin_rewards.control_display_names.name_en'
        );
      case 'name_bg':
        return this.translateService.instant(
          'admin_rewards.control_display_names.name_bg'
        );
      case 'description_en':
        return this.translateService.instant(
          'admin_rewards.control_display_names.description_en'
        );
      case 'description_bg':
        return this.translateService.instant(
          'admin_rewards.control_display_names.description_bg'
        );
      case 'requiredPoints':
        return this.translateService.instant(
          'admin_rewards.control_display_names.required_points'
        );
      case 'image':
        return this.translateService.instant(
          'admin_rewards.control_display_names.image'
        );
      case 'cashEquivalent':
        return this.translateService.instant(
          'admin_rewards.control_display_names.cash_equivalent'
        );
      default:
        return controlName;
    }
  }

  private fetchSystemRewardList(searchExpression: string = '') {
    this.rewardsService.getList(searchExpression).subscribe({
      next: (rewardList) => {
        this.rewardList = rewardList ?? [];
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

  private initFormGroup(): FormGroup {
    return this.fb.group({
      selectedLevel: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      requiredPoints: new FormControl<number>({ value: 0, disabled: true }, [
        Validators.required,
      ]),
      name_en: new FormControl<string>('', [Validators.required]),
      name_bg: new FormControl<string>('', [Validators.required]),
      cashEquivalent: new FormControl<number>(0, [Validators.required]),
      description_en: new FormControl<string>('', [Validators.required]),
      description_bg: new FormControl<string>('', [Validators.required]),
      image: new FormControl<string | null>('', [Validators.required]),
    });
  }

  private resetRewardForm(): void {
    // emitEvent: false - this reset is programmatic form teardown, not user
    // input; letting it through valueChanges would auto-save an empty draft.
    this.form.reset(undefined, { emitEvent: false });
    this.imagePreview = null;
    this.fileToUpload = null;
    this.imageError = null;
    this.editRewardId = null;
    this.rewardRequiredPointList = [];
    this.skipFirstSelectedLevelChange = true;
    this.form.controls.requiredPoints.disable({ emitEvent: false });
  }
}
