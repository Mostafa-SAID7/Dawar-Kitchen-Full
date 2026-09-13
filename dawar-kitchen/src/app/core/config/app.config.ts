import { ApplicationConfig, isDevMode, importProvidersFrom } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withPreloading, PreloadingStrategy, Route } from '@angular/router';
import { environment } from '../../../environments/environment';
import { provideHttpClient, withFetch, withInterceptors, HttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideServiceWorker } from '@angular/service-worker';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { routes } from './app.routes';
import { authInterceptor, errorInterceptor, languageInterceptor } from '../interceptors';
import { NgZone, Injectable } from '@angular/core';

// ── Optimized multi-namespace i18n loader with HTTP caching
// Loads from /assets/i18n/en/ and /assets/i18n/ar/ (original structure)
// Browser HTTP cache + Service Worker datagroups handle request deduplication
export class OptimizedMultiTranslateHttpLoader implements TranslateLoader {
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
  return new OptimizedMultiTranslateHttpLoader(http, '/assets/i18n/');
}

// ── Selective preloading strategy: only preload marked routes after idle time
@Injectable({ providedIn: 'root' })
export class SelectivePreloadingStrategy implements PreloadingStrategy {
  private preloading = false;

  constructor(private ngZone: NgZone) {
    if (typeof requestIdleCallback !== 'undefined') {
      this.ngZone.runOutsideAngular(() => {
        requestIdleCallback(() => {
          this.preloading = true;
        }, { timeout: 2000 });
      });
    } else {
      setTimeout(() => { this.preloading = true; }, 2000);
    }
  }

  preload(route: Route, load: () => Observable<any>): Observable<any> {
    if (route.data && route.data['preload'] && this.preloading) {
      return load();
    }
    return of(null);
  }
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
      withPreloading(SelectivePreloadingStrategy)
    ),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor, errorInterceptor, languageInterceptor])),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000'
    }),
    SelectivePreloadingStrategy,
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
