import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Pipe({
  name: 'translateIn',
})
export class TranslateInPipe implements PipeTransform {
  constructor(private translate: TranslateService, private http: HttpClient) {}

  public transform(key: string, lang: string): Observable<string> {
    const translateIn = this.translate.getLangs().includes(lang)
      ? lang
      : this.translate.getCurrentLang();
    return this.http.get(`shared/assets/i18n/${translateIn}.json`).pipe(
      map((res) => {
        let translation = res;
        key.split('.').forEach((k) => {
          translation = translation[k];
        });
        return translation as string;
      })
    );
  }
}
