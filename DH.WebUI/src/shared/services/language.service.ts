import {
  InterpolatableTranslationObject,
  TranslateService,
} from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { SupportLanguages } from '../../entities/common/models/support-languages.enum';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private language$ = new BehaviorSubject<SupportLanguages>(
    SupportLanguages.EN
  );

  constructor(private readonly translate: TranslateService) {
    const supportedLanguages = Object.keys(SupportLanguages)
      .filter((key) => isNaN(Number(key)))
      .map((key) => key.toLowerCase());
    this.translate.addLangs(supportedLanguages);

    this.translate.setFallbackLang(this.getLanguageCode(SupportLanguages.EN));
  }

  public getLanguageCode(lang: SupportLanguages): string {
    return lang.toString().toLowerCase();
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

  public setLanguage$(
    lang: SupportLanguages
  ): Observable<InterpolatableTranslationObject> {
    const currentLang = this.getLanguageCode(lang);
    let useLang$: Observable<InterpolatableTranslationObject>;

    if (this.translate.getLangs().includes(currentLang)) {
      useLang$ = this.translate.use(currentLang);
    } else {
      useLang$ = this.translate.use(this.getLanguageCode(SupportLanguages.EN));
      lang = SupportLanguages.EN;
    }

    return useLang$.pipe(tap(() => this.language$.next(lang)));
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
