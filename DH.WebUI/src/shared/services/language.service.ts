import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { TenantUserSettingsService } from '../../entities/common/api/tenant-user-settings.service';
import { SupportLanguages } from '../../entities/common/models/support-languages.enum';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private language$ = new BehaviorSubject<SupportLanguages>(
    SupportLanguages.EN
  );

  constructor(
    private readonly translate: TranslateService,
    private readonly tenantUserSettingsService: TenantUserSettingsService
  ) {
    const supportedLanguages = Object.keys(SupportLanguages)
      .filter((key) => isNaN(Number(key)))
      .map((key) => key.toLowerCase());
    this.translate.addLangs(supportedLanguages);

    this.translate.setFallbackLang(this.getLanguageCode(SupportLanguages.EN));
  }

  public getLanguageCode(lang: SupportLanguages): string {
    return lang.toString().toLowerCase();
  }

  public loadUserLanguage(): void {
    this.tenantUserSettingsService
      .get()
      .pipe(
        tap((res) => {
          const lang = res?.language || SupportLanguages.EN;

          this.setLanguage(lang);
        })
      )
      .subscribe({
        error: () => {
          this.setLanguage(SupportLanguages.EN);
        },
      });
  }

  public setLanguage(lang: SupportLanguages) {
    const currentLang = this.getLanguageCode(lang);

    if (this.translate.getLangs().includes(currentLang)) {
      this.translate.use(currentLang);
      this.language$.next(lang);
    } else {
      this.translate.use(this.getLanguageCode(SupportLanguages.EN));
      this.language$.next(SupportLanguages.EN);
    }
  }

  public instant(key: string): string {
    return this.translate.instant(key);
  }

  /**
   * Get observable for current language
   */
  public getLanguageObservable() {
    return this.language$.asObservable();
  }

  /**
   * Get current language value
   */
  public getCurrentLanguage(): SupportLanguages {
    return this.language$.value;
  }
}
