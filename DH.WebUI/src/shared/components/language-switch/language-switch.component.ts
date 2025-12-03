import { Component, OnInit } from '@angular/core';
import { LanguageService } from '../../services/language.service';
import { SupportLanguages } from '../../../entities/common/models/support-languages.enum';

@Component({
    selector: 'app-language-switch',
    templateUrl: './language-switch.component.html',
    styleUrls: ['./language-switch.component.scss'],
    standalone: false
})
export class LanguageSwitchComponent implements OnInit {
  public currentLang!: string;

  constructor(private languageService: LanguageService) {}

  public ngOnInit(): void {
    this.currentLang = this.languageService.getCurrentLanguage();
  }

  public changeLang(lang: string) {
    this.languageService.setLanguage(lang as SupportLanguages);
    this.currentLang = lang;
  }

  public isActive(lang: string): boolean {
    return this.currentLang === lang;
  }
}
