import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { environment } from './shared/environments/environment.development';

console.log('app be url -> ',environment.defaultAppUrl);

try {
  platformBrowserDynamic().bootstrapModule(AppModule)
    .then(() => console.log('AppModule bootstrapped'))
    .catch(err => console.error('Async bootstrap error:', err));
} catch (err) {
  console.error('Sync bootstrap error:', err);
}
