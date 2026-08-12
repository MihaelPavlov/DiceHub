import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import {
  ICreateTenantResult,
  TenantProvisioningService,
} from '../../../../entities/common/api/tenant-provisioning.service';
import { Form } from '../../../../shared/components/form/form.component';
import { Formify } from '../../../../shared/models/form.model';
import { ROUTE } from '../../../../shared/configs/route.config';
import { LoadingService } from '../../../../shared/services/loading.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { ToastType } from '../../../../shared/models/toast.model';
import { AuthService } from '../../../../entities/auth/auth.service';

interface IProvisionForm {
  tenantId: string;
  tenantName: string;
  town: string;
  ownerEmail: string;
  clubPhoneNumber: string;
  logoFileName: string;
  starterProfile: string;
}

@Component({
  selector: 'app-provision-tenant',
  templateUrl: './provision-tenant.component.html',
  styleUrl: './provision-tenant.component.scss',
  standalone: false,
})
export class ProvisionTenantComponent extends Form {
  override form: Formify<IProvisionForm>;
  public result: ICreateTenantResult | null = null;

  public readonly starterProfiles = [
    { value: 'starter-pack', label: 'Starter Pack (games, challenges, rewards)' },
    { value: 'empty-club', label: 'Empty Club (blank slate)' },
  ];

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly provisioningService: TenantProvisioningService,
    private readonly loadingService: LoadingService,
    private readonly authService: AuthService,
    public override readonly toastService: ToastService,
    public override readonly translateService: TranslateService
  ) {
    super(toastService, translateService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) this.getServerErrorMessage = null;
    });
  }

  public onProvision(): void {
    if (!this.form.valid) return;

    this.loadingService.loadingOn();

    this.provisioningService
      .provision({
        tenantId: this.form.controls.tenantId.value.trim(),
        tenantName: this.form.controls.tenantName.value.trim(),
        town: this.form.controls.town.value.trim(),
        ownerEmail: this.form.controls.ownerEmail.value.trim(),
        clubPhoneNumber: this.form.controls.clubPhoneNumber.value.trim(),
        logoFileName: this.form.controls.logoFileName.value?.trim() ?? '',
        starterProfile: this.form.controls.starterProfile.value,
      })
      .subscribe({
        next: (result) => {
          this.loadingService.loadingOff();
          this.result = result;
          this.toastService.success({
            message: 'Tenant provisioned successfully!',
            type: ToastType.Success,
          });
        },
        error: (error) => {
          this.loadingService.loadingOff();
          this.handleServerErrors(error);
          this.toastService.error({
            message: 'Failed to provision tenant.',
            type: ToastType.Error,
          });
        },
      });
  }

  public provisionAnother(): void {
    this.result = null;
    this.form.reset({ starterProfile: 'starter-pack' });
  }

  public navigateToChooseClub(): void {
    this.router.navigate([ROUTE.CHOOSE_CLUB]);
  }

  public logout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigate([ROUTE.LANDING]);
    });
  }

  protected override getControlDisplayName(controlName: string): string {
    const names: Record<string, string> = {
      tenantId: 'Tenant ID',
      tenantName: 'Club Name',
      town: 'Town',
      ownerEmail: 'Owner Email',
      clubPhoneNumber: 'Club Phone',
      logoFileName: 'Logo File Name',
      starterProfile: 'Starter Profile',
    };
    return names[controlName] ?? controlName;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      tenantId: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      tenantName: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      town: new FormControl<string>('', [Validators.required]),
      ownerEmail: new FormControl<string>('', [Validators.required]),
      clubPhoneNumber: new FormControl<string>('', [Validators.required]),
      logoFileName: new FormControl<string>(''),
      starterProfile: new FormControl<string>('starter-pack', [
        Validators.required,
      ]),
    });
  }
}
