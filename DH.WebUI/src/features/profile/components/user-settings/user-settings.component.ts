import { Component, OnDestroy, OnInit } from '@angular/core';
import { Form } from '../../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { MenuTabsService } from '../../../../shared/services/menu-tabs.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { NAV_ITEM_LABELS } from '../../../../shared/models/nav-items-labels.const';
import { Formify } from '../../../../shared/models/form.model';
import { TenantUserSettingsService } from '../../../../entities/common/api/tenant-user-settings.service';
import { AppToastMessage } from '../../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../../shared/models/toast.model';

interface IUserSettingsForm {
  phoneNumber: string;
}

@Component({
  selector: 'app-user-settings',
  templateUrl: 'user-settings.component.html',
  styleUrl: 'user-settings.component.scss',
})
export class UserSettingsComponent extends Form implements OnInit, OnDestroy {
  override form: Formify<IUserSettingsForm>;
  public tenantSettingsId: number | null = null;
  public isMenuVisible: boolean = false;

  constructor(
    private readonly fb: FormBuilder,
    public override readonly toastService: ToastService,
    private readonly menuTabsService: MenuTabsService,
    private readonly userSettingService: TenantUserSettingsService,
    private readonly router: Router
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
    this.menuTabsService.setActive(NAV_ITEM_LABELS.PROFILE);
  }
  public ngOnDestroy(): void {
    this.menuTabsService.resetData();
  }

  public ngOnInit(): void {
    this.fetchSettings();
  }

  public fetchSettings(): void {
    this.userSettingService.get().subscribe({
      next: (res) => {
        this.tenantSettingsId = res.id ?? null;
        this.form.patchValue({
          phoneNumber: res.phoneNumber,
        });
      },
    });
  }

  public onSave(): void {
    if (this.form.valid) {
      this.userSettingService
        .update({
          id: this.tenantSettingsId,
          phoneNumber: this.form.controls.phoneNumber.value,
        })
        .subscribe({
          next: () => {
            this.toastService.success({
              message: AppToastMessage.ChangesSaved,
              type: ToastType.Success,
            });

            this.fetchSettings();
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

  public showMenu(): void {
    this.isMenuVisible = !this.isMenuVisible;
  }

  public backNavigateBtn() {
    this.router.navigateByUrl('profile');
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'averageMaxCapacity':
        return 'Average Max Capacity';
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage(): void {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      id: new FormControl<number | null>(null),
      phoneNumber: new FormControl<string | null>('', [Validators.required]),
    });
  }
}
