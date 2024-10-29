import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../entities/auth/auth.service';
import { Router } from '@angular/router';
import { MessagingService } from '../../../entities/messaging/api/messaging.service';
import { Form } from '../../../shared/components/form/form.component';
import { ToastService } from '../../../shared/services/toast.service';
import { Formify } from '../../../shared/models/form.model';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';

interface ILoginForm {
  email: string;
  password: string;
  rememberMe: boolean;
}

@Component({
  selector: 'app-login',
  templateUrl: 'login.component.html',
  styleUrl: 'login.component.scss',
})
export class LoginComponent extends Form implements OnInit {
  override form: Formify<ILoginForm>;

  constructor(
    private readonly router: Router,
    readonly authService: AuthService,
    private readonly messagingService: MessagingService,
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

  public ngOnInit(): void {
  
  }

  private clearServerErrorMessage() {
    this.getServerErrorMessage = null;
  }

  protected override getControlDisplayName(controlName: string): string {
    throw new Error('Method not implemented.');
  }

  public navigateToGameDetails(): void {
    this.router.navigateByUrl('games/1/details');
  }

  public onLogin():void{
    if(this.form.valid){
      this.authService.login({
        email: this.form.controls.email,
        password: this.form.controls.password,
      });
    }
  }

  loginUser() {
    this.authService.login({
      email: 'rap4obg2@abv.bg',
      password: '123456789Mm!',
    });
  }

  loginUser2() {
    this.authService.login({
      email: 'rap4obg3@abv.bg',
      password: '123456789Mm!',
    });
  }

  loginUser3() {
    this.authService.login({
      email: 'rap4obg4@abv.bg',
      password: '123456789Mm!',
    });
  }
  loginUser4() {
    this.authService.login(
      {
        email: 'rap4obg17@abv.bg',
        password: '123456789Mm!',
      },
      true
    );
  }

  loginAdmin() {
    this.authService.login({ email: 'sa@dicehub.com', password: '1qaz!QAZ' });
  }
  game() {
    this.authService.game({ name: 'test123' });
  }
  register() {
    this.messagingService
      .getDeviceTokenForRegistration()
      .then((deviceToken) => {
        console.log('from register -> ', deviceToken);

        this.authService
          .register({
            username: 'rap4obg17',
            email: 'rap4obg17@abv.bg',
            password: '123456789Mm!',
            deviceToken,
          })
          .subscribe({
            next: () => {
              this.loginUser4();
            },
          });
      });
  }
  userInfo() {
    this.authService.userinfo();
  }
  logout() {
    this.authService.logout();
  }

  private initFormGroup(): FormGroup {
    return this.fb.group({
      email: new FormControl<string>('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      password: new FormControl<string>('', Validators.required),
      rememberMe: new FormControl<boolean>(true, Validators.required),
    });
  }
}
