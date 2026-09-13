import { ApplicationConfig, isDevMode, importProvidersFrom } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withPreloading, PreloadAllModules } from '@angular/router';
import { environment } from '../../../environments/environment';
import { DEFAULT_LANGUAGE } from '../../shared/constants';
import { provideHttpClient, withFetch, withInterceptors, HttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideServiceWorker } from '@angular/service-worker';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { routes } from './app.routes';
import { authInterceptor, errorInterceptor, languageInterceptor } from '../interceptors';

export class MultiTranslateHttpLoader implements TranslateLoader {
  private readonly files = ['common', 'home', 'menu', 'reservations', 'auth', 'contact', 'payment'];

  constructor(private readonly http: HttpClient, private readonly prefix = '/assets/i18n/') {}

  public getTranslation(lang: string): Observable<any> {
    const requests = this.files.map(file =>
      this.http.get(`${this.prefix}${lang}/${file}.json`, {
        params: { v: environment.i18nVersion }
      }).pipe(catchError(() => of({})))
    );

    return forkJoin(requests).pipe(
      map(responses => Object.assign({}, ...responses))
    );
  }
}

export function HttpLoaderFactory(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, '/assets/i18n/');
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled'
      }),
      withPreloading(PreloadAllModules)
    ),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor, languageInterceptor])),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000'
    }),
    // ✅ TranslateModule for i18n (standalone configuration)
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: DEFAULT_LANGUAGE,
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    )
  ]
};
