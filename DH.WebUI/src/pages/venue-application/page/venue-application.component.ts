import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { TenantApplicationsService } from '../../../entities/common/api/tenant-applications.service';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { LanguageService } from '../../../shared/services/language.service';
import { downscaleImageFile } from '../../../shared/helpers/image-resize.helper';
import { ToastService } from '../../../shared/services/toast.service';
import { FormDraftService, IFormDraftOptions } from '../../../shared/services/form-draft.service';

interface IVenueApplicationForm {
  contactName: string;
  email: string;
  emailVerificationCode: string;
  phoneNumber: string;
  address: string;
  publicWebsite: string;
  socialPage: string;
  discordServer: string;
  photoUrl: string;
}

interface IVenueApplicationDraftExtra {
  isEmailVerified: boolean;
  isEmailCodeSent: boolean;
}

@Component({
  selector: 'app-venue-application',
  templateUrl: 'venue-application.component.html',
  styleUrl: 'venue-application.component.scss',
  standalone: false,
})
export class VenueApplicationComponent implements OnDestroy {
  public form: FormGroup;
  public isEmailVerified = false;
  public isEmailCodeSent = false;
  public isSendingEmailCode = false;
  public isVerifyingEmailCode = false;
  public isSubmitted = false;
  public isSaving = false;
  public serverErrors: string[] = [];
  public logoFile: File | null = null;
  public logoPreviewUrl: string | null = null;
  public logoError: string | null = null;

  private static readonly MaxLogoSizeBytes = 2 * 1024 * 1024;
  private static readonly AllowedLogoTypes = ['image/png', 'image/jpeg', 'image/webp', 'image/svg+xml'];
  private static readonly DraftKey = 'venueApplication';
  private draftSubscription: Subscription | null = null;
  private readonly draftOptions: IFormDraftOptions<IVenueApplicationDraftExtra> = {
    getExtra: () => ({ isEmailVerified: this.isEmailVerified, isEmailCodeSent: this.isEmailCodeSent }),
    applyExtra: (extra) => {
      this.isEmailCodeSent = extra.isEmailCodeSent;
      if (extra.isEmailVerified) {
        this.isEmailVerified = true;
        this.form.get('email')?.disable();
        this.form.get('emailVerificationCode')?.disable();
      }
    },
  };

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly languageService: LanguageService,
    private readonly toastService: ToastService,
    private readonly translateService: TranslateService,
    private readonly formDraftService: FormDraftService
  ) {
    this.form = this.initFormGroup();
    this.draftSubscription = this.formDraftService.autoSave(
      this.form,
      VenueApplicationComponent.DraftKey,
      this.draftOptions
    );
    this.listenForContactChanges();
  }

  /**
   * isEmailCodeSent/isEmailVerified flip inside async HTTP success callbacks, not via
   * any form control value change - the debounced autosave (keyed off form.valueChanges)
   * never fires for them on its own. Without this explicit save, clicking "Send Code"
   * then backgrounding the app before typing anything else would restore to a stale
   * draft that still thinks no code was ever sent.
   */
  private saveDraftNow(): void {
    this.formDraftService.save(this.form, VenueApplicationComponent.DraftKey, this.draftOptions);
  }

  public sendEmailVerificationCode(): void {
    this.serverErrors = [];
    const emailControl = this.form.get('email');

    if (!emailControl || emailControl.invalid) {
      emailControl?.markAsTouched();
      this.serverErrors = [
        this.translateService.instant('venue_application.errors.invalid_email'),
      ];
      return;
    }

    this.isSendingEmailCode = true;
    this.tenantApplicationsService
      .sendEmailVerificationCode({
        email: emailControl.value,
        language: this.languageService.getCurrentLanguage(),
      })
      .subscribe({
        next: (isSent) => {
          if (!isSent) {
            this.serverErrors = [
              this.translateService.instant('venue_application.errors.email_code_not_sent'),
            ];
            return;
          }

          this.isEmailCodeSent = true;
          this.saveDraftNow();
          this.toastService.success({
            message: this.translateService.instant('venue_application.toasts.email_code_sent'),
            type: ToastType.Success,
          });
        },
        error: () => {
          this.serverErrors = [
            this.translateService.instant('venue_application.errors.email_code_not_sent'),
          ];
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
        complete: () => {
          this.isSendingEmailCode = false;
        },
      });
  }

  public verifyEmailCode(): void {
    this.serverErrors = [];
    const emailControl = this.form.get('email');
    const codeControl = this.form.get('emailVerificationCode');

    if (!emailControl || !codeControl || emailControl.invalid || codeControl.invalid) {
      emailControl?.markAsTouched();
      codeControl?.markAsTouched();
      this.serverErrors = [
        this.translateService.instant('venue_application.errors.email_code_required'),
      ];
      return;
    }

    this.isVerifyingEmailCode = true;
    this.tenantApplicationsService
      .verifyEmailCode({
        email: emailControl.value,
        code: codeControl.value,
      })
      .subscribe({
        next: (isVerified) => {
          if (!isVerified) {
            this.serverErrors = [
              this.translateService.instant('venue_application.errors.invalid_code'),
            ];
            return;
          }

          this.isEmailVerified = true;
          emailControl.disable();
          codeControl.disable();
          this.saveDraftNow();
          this.toastService.success({
            message: this.translateService.instant('venue_application.toasts.email_verified'),
            type: ToastType.Success,
          });
        },
        error: () => {
          this.serverErrors = [
            this.translateService.instant('venue_application.errors.email_verification_failed'),
          ];
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
        },
        complete: () => {
          this.isVerifyingEmailCode = false;
        },
      });
  }

  public async onLogoSelected(event: Event): Promise<void> {
    this.logoError = null;
    const input = event.target as HTMLInputElement;
    const original = input.files?.[0] ?? null;

    if (!original) {
      return;
    }

    if (!VenueApplicationComponent.AllowedLogoTypes.includes(original.type)) {
      this.logoError = this.translateService.instant('venue_application.errors.logo_invalid_type');
      input.value = '';
      return;
    }

    if (original.size > VenueApplicationComponent.MaxLogoSizeBytes) {
      this.logoError = this.translateService.instant('venue_application.errors.logo_too_large');
      input.value = '';
      return;
    }

    // Preserve PNG transparency; SVG passes through untouched (helper handles it).
    const file = await downscaleImageFile(original, {
      mimeType: original.type === 'image/png' ? 'image/png' : 'image/jpeg',
    });

    this.removeLogo();
    this.logoFile = file;
    this.logoPreviewUrl = URL.createObjectURL(file);
  }

  public removeLogo(): void {
    if (this.logoPreviewUrl) {
      URL.revokeObjectURL(this.logoPreviewUrl);
    }
    this.logoFile = null;
    this.logoPreviewUrl = null;
  }

  public submit(): void {
    this.serverErrors = [];

    if (
      this.form.invalid ||
      !this.isEmailVerified ||
      !this.hasPublicProof()
    ) {
      this.form.markAllAsTouched();
      if (!this.hasPublicProof()) {
        this.serverErrors = [
          this.translateService.instant('venue_application.errors.public_proof_required'),
        ];
      }
      return;
    }

    this.isSaving = true;
    const value = this.form.getRawValue() as IVenueApplicationForm;

    this.tenantApplicationsService
      .create(
        {
          applicantType: 'Venue/Club',
          contactName: value.contactName,
          email: value.email,
          phoneNumber: value.phoneNumber,
          isEmailVerified: this.isEmailVerified,
          isPhoneVerified: true,
          address: value.address,
          publicWebsite: value.publicWebsite ?? '',
          socialPage: value.socialPage ?? '',
          discordServer: value.discordServer ?? '',
          photoUrl: value.photoUrl ?? '',
        },
        this.logoFile
      )
      .subscribe({
        next: () => {
          this.isSubmitted = true;
          this.formDraftService.clear(VenueApplicationComponent.DraftKey);
          this.toastService.success({
            message: this.translateService.instant('venue_application.toasts.application_submitted'),
            type: ToastType.Success,
          });
        },
        error: (error) => {
          this.serverErrors = this.extractErrors(error);
          this.toastService.error({
            message: AppToastMessage.SomethingWrong,
            type: ToastType.Error,
          });
          this.isSaving = false;
        },
        complete: () => {
          this.isSaving = false;
        },
      });
  }

  public navigateToLanding(): void {
    this.router.navigateByUrl('/');
  }

  public ngOnDestroy(): void {
    if (this.logoPreviewUrl) {
      URL.revokeObjectURL(this.logoPreviewUrl);
    }
    this.draftSubscription?.unsubscribe();
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      contactName: new FormControl<string>('', [Validators.required, Validators.maxLength(100)]),
      email: new FormControl<string>('', [Validators.required, Validators.email]),
      emailVerificationCode: new FormControl<string>('', [Validators.required, Validators.pattern(/^[0-9]{6}$/)]),
      phoneNumber: new FormControl<string>('', [Validators.required, Validators.maxLength(20), Validators.pattern(/^\+?[0-9\s-]{7,20}$/)]),
      address: new FormControl<string>('', [Validators.required, Validators.maxLength(300)]),
      publicWebsite: new FormControl<string>(''),
      socialPage: new FormControl<string>(''),
      discordServer: new FormControl<string>(''),
      photoUrl: new FormControl<string>('', [Validators.maxLength(1000)]),
    });
  }

  private extractErrors(error: any): string[] {
    const errors = error?.error?.errors;
    if (!errors) {
      return [this.translateService.instant('venue_application.errors.application_failed')];
    }

    return Object.values(errors).flat() as string[];
  }

  public hasPublicProof(): boolean {
    const value = this.form.getRawValue() as IVenueApplicationForm;

    return !!(
      value.publicWebsite?.trim() ||
      value.socialPage?.trim() ||
      value.discordServer?.trim()
    );
  }

  public canShowVenueDetails(): boolean {
    return this.isEmailVerified;
  }

  private listenForContactChanges(): void {
    this.form.get('email')?.valueChanges.subscribe(() => {
      if (this.isEmailVerified) return;

      this.isEmailCodeSent = false;
      this.form.get('emailVerificationCode')?.reset('');
    });
  }
}
