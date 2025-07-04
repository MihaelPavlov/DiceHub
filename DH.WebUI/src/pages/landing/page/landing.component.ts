import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ROUTE } from '../../../shared/configs/route.config';
import { Form } from '../../../shared/components/form/form.component';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { PartnerInquiriesService } from '../../../entities/common/api/partner-inquiries.service';
import { ToastType } from '../../../shared/models/toast.model';
import { AppToastMessage } from '../../../shared/components/toast/constants/app-toast-messages.constant';

interface IPartnerInquiryForm {
  name: string;
  email: string;
  message: string;
  phoneNumber: string;
}

@Component({
  selector: 'app-landing',
  templateUrl: 'landing.component.html',
  styleUrl: 'landing.component.scss',
})
export class LandingComponent extends Form {
  override form: Formify<IPartnerInquiryForm>;

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly partnerInquiriesService: PartnerInquiriesService,
    public override readonly toastService: ToastService
  ) {
    super(toastService);

    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
  }
  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }
  public onLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public onRegister(): void {
    this.router.navigateByUrl(ROUTE.REGISTER);
  }

  public onInstructions(): void {
    this.router.navigateByUrl(ROUTE.INSTRUCTIONS);
  }
  public onSubmit(): void {
    if (this.form.valid) {
      this.partnerInquiriesService
        .create({
          name: this.form.controls.name.value,
          email: this.form.controls.email.value,
          phoneNumber: this.form.controls.phoneNumber.value,
          message: this.form.controls.message.value,
        })
        .subscribe({
          next: (response) => {
            this.toastService.success({
              message: 'Inquiry submitted successfully!',
              type: ToastType.Success,
            });
            this.form.reset();
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
      case 'name':
        return 'Name';
      case 'email':
        return 'Email';
      case 'message':
        return 'Message';
      case 'phoneNumber':
        return 'Phone Number';
      default:
        return controlName;
    }
  }
  private initFormGroup(): FormGroup {
    return this.fb.group({
      name: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      email: new FormControl<string>('', [Validators.required]),
      phoneNumber: new FormControl<string>('', [Validators.required]),
      message: new FormControl<string>('', [Validators.required]),
    });
  }
}
