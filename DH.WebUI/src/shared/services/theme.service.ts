import { Injectable } from '@angular/core';
import { UiTheme } from '../enums/ui-theme.enum';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly darkClass = 'theme-dark';
  private readonly lightClass = 'theme-light';

  public applyTheme(uiTheme: UiTheme): void {
    this.apply(UiTheme.Dark == uiTheme ? this.darkClass : this.lightClass);
  }

  private apply(themeClass: string): void {
    document.body.classList.remove(this.darkClass, this.lightClass);
    document.body.classList.add(themeClass);
  }
}
