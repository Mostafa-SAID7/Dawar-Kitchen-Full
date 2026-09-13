import { ApplicationConfig, isDevMode, importProvidersFrom } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withPreloading, PreloadAllModules } from '@angular/router';
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

  constructor(private readonly http: HttpClient, private readonly prefix = './assets/i18n/') {}

  public getTranslation(lang: string): Observable<any> {
    // Cache-bust with build timestamp to ensure fresh translations after deployments
    const v = (window as any).__i18n_v || (((window as any).__i18n_v = Date.now()), (window as any).__i18n_v);
    const requests = this.files.map(file =>
      this.http.get(`${this.prefix}${lang}/${file}.json`, { params: { _v: v } }).pipe(
        catchError((err) => { console.error(`[i18n] Failed to load ${lang}/${file}.json`, err); return of({}); })
      )
    );
    return forkJoin(requests).pipe(
      map(responses => {
        const merged = Object.assign({}, ...responses);
        console.log(`[i18n] Loaded ${lang} translations:`, Object.keys(merged), 'nav.home =', merged?.nav?.home);
        return merged;
      })
    );
  }
}

export function HttpLoaderFactory(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, './assets/i18n/');
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
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    )
  ]
};
