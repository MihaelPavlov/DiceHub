import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TenantApplicationsService } from '../../../entities/common/api/tenant-applications.service';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';
import { ToastType } from '../../../shared/models/toast.model';
import { LanguageService } from '../../../shared/services/language.service';
import { ToastService } from '../../../shared/services/toast.service';

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

@Component({
  selector: 'app-venue-application',
  templateUrl: 'venue-application.component.html',
  styleUrl: 'venue-application.component.scss',
  standalone: false,
})
export class VenueApplicationComponent {
  public form: FormGroup;
  public isEmailVerified = false;
  public isEmailCodeSent = false;
  public isSendingEmailCode = false;
  public isVerifyingEmailCode = false;
  public isSubmitted = false;
  public isSaving = false;
  public serverErrors: string[] = [];

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly tenantApplicationsService: TenantApplicationsService,
    private readonly languageService: LanguageService,
    private readonly toastService: ToastService
  ) {
    this.form = this.initFormGroup();
    this.listenForContactChanges();
  }

  public sendEmailVerificationCode(): void {
    this.serverErrors = [];
    const emailControl = this.form.get('email');

    if (!emailControl || emailControl.invalid) {
      emailControl?.markAsTouched();
      this.serverErrors = ['Enter a valid email before sending the code.'];
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
            this.serverErrors = ['Email verification code was not sent.'];
            return;
          }

          this.isEmailCodeSent = true;
          this.toastService.success({
            message: 'Email verification code sent.',
            type: ToastType.Success,
          });
        },
        error: () => {
          this.serverErrors = ['Email verification code was not sent.'];
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
      this.serverErrors = ['Enter the email code before verifying.'];
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
            this.serverErrors = ['Invalid or expired email verification code.'];
            return;
          }

          this.isEmailVerified = true;
          emailControl.disable();
          codeControl.disable();
          this.toastService.success({
            message: 'Email verified.',
            type: ToastType.Success,
          });
        },
        error: () => {
          this.serverErrors = ['Email verification failed.'];
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
          'Provide at least one public website, social page, or Discord server.',
        ];
      }
      return;
    }

    this.isSaving = true;
    const value = this.form.getRawValue() as IVenueApplicationForm;

    this.tenantApplicationsService
      .create({
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
      })
      .subscribe({
        next: () => {
          this.isSubmitted = true;
          this.toastService.success({
            message: 'Venue application submitted.',
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
    if (!errors) return ['Application submission failed.'];

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
