import { LanguageService } from './../../services/language.service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { ExceptionBaseComponent } from '../base/exception-base.component';

@Component({
  selector: 'app-not-found',
  templateUrl: 'not-found.component.html',
  styleUrls: ['not-found.component.scss'],
})
export class NotFoundComponent extends ExceptionBaseComponent {
  protected imageCode = '404';

  constructor(
    router: Router,
    authService: AuthService,
    languageService: LanguageService
  ) {
    super(router, authService, languageService);
  }
}
