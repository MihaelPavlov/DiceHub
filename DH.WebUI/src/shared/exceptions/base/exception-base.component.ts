import { Router } from '@angular/router';
import { AuthService } from '../../../entities/auth/auth.service';
import { LanguageService } from '../../services/language.service';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';
import { FULL_ROUTE, ROUTE } from '../../configs/route.config';

export abstract class ExceptionBaseComponent {
  protected abstract imageCode: string; 

  constructor(
    protected readonly router: Router,
    protected readonly authService: AuthService,
    protected readonly languageService: LanguageService
  ) {}

  public redirectTo(): void {
    if (this.authService.getUser)
      this.router.navigateByUrl(FULL_ROUTE.GAMES.LIBRARY);
    else
      this.router.navigateByUrl(ROUTE.LOGIN);
  }

  public get imgPath(): string {
    const language = this.languageService.getCurrentLanguage();
    const langSuffix = language === SupportLanguages.BG ? 'bg' : 'en';
    return `shared/assets/images/exceptions/${this.imageCode}_${langSuffix}.jpg`;
  }
}
