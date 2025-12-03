import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { LanguageService } from '../../services/language.service';
import { ExceptionBaseComponent } from '../base/exception-base.component';

@Component({
    selector: 'app-server-error',
    templateUrl: 'server-error.component.html',
    styleUrls: ['server-error.component.scss'],
    standalone: false
})
export class ServerErrorComponent extends ExceptionBaseComponent {
  protected imageCode = '500';

  constructor(
    router: Router,
    authService: AuthService,
    languageService: LanguageService
  ) {
    super(router, authService, languageService);
  }
}
