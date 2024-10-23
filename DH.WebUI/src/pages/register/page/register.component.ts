import { Component, OnInit } from '@angular/core';
import { Form } from '../../../shared/components/form/form.component';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';
import { Formify } from '../../../shared/models/form.model';
import { ToastService } from '../../../shared/services/toast.service';

interface IRegisterForm {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-register',
  templateUrl: 'register.component.html',
  styleUrl: 'register.component.scss',
})
export class RegisterComponent extends Form implements OnInit {
  override form: Formify<IRegisterForm>;

  constructor(
    private readonly router: Router,
    readonly authService: AuthService,
    public override readonly toastService: ToastService,
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

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  public ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      username: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      email: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      password: new FormControl<string>('', Validators.required),
      confirmPassword: new FormControl<string>('', Validators.required),
    });
  }
}
