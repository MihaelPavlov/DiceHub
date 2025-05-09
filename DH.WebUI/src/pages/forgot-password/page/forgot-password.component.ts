import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';
import { Form } from '../../../shared/components/form/form.component';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';
import { ROUTE } from '../../../shared/configs/route.config';

interface IForgotPasswordForm {
  email: string;
}

@Component({
  selector: 'app-login',
  templateUrl: 'forgot-password.component.html',
  styleUrl: 'forgot-password.component.scss',
})
export class ForgotPasswordComponent extends Form {
  override form: Formify<IForgotPasswordForm>;
  constructor(
    private readonly router: Router,
    readonly authService: AuthService,
    public override readonly toastService: ToastService,
    private readonly messagingService: MessagingService,
    private readonly fb: FormBuilder
  ) {
    super(toastService);
    this.form = this.initFormGroup();
    this.form.valueChanges.subscribe(() => {
      if (this.getServerErrorMessage) {
        this.clearServerErrorMessage();
      }
    });
  }

  public onSubmit(): void {}

  public navigateToLogin(): void {
    this.router.navigateByUrl(ROUTE.LOGIN);
  }

  protected override getControlDisplayName(controlName: string): string {
    switch (controlName) {
      case 'email':
        return 'Email';
      default:
        return controlName;
    }
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [Validators.required]),
    });
  }
}
