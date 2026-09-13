import { ApplicationConfig, isDevMode, importProvidersFrom } from '@angular/core';
import { provideRouter, withInMemoryScrolling, withPreloading, PreloadingStrategy, Route } from '@angular/router';
import { environment } from '../../../environments/environment';
import { provideHttpClient, withFetch, withInterceptors, HttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideServiceWorker } from '@angular/service-worker';
import { Observable, forkJoin, of, timer } from 'rxjs';
import { map, catchError, mergeMap } from 'rxjs/operators';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { routes } from './app.routes';
import { authInterceptor, errorInterceptor, languageInterceptor } from '../interceptors';
import { NgZone, Injectable } from '@angular/core';

// ── Merged i18n loader (1 request per language instead of 7)
export class OptimizedTranslateLoader implements TranslateLoader {
  constructor(private readonly http: HttpClient, private readonly prefix = '/assets/i18n/') {}

  public getTranslation(lang: string): Observable<any> {
    return this.http.get(`${this.prefix}${lang}.json`, {
      params: { v: environment.i18nVersion }
    }).pipe(catchError(() => of({})));
  }
}

export function HttpLoaderFactory(http: HttpClient) {
  return new OptimizedTranslateLoader(http, '/assets/i18n/');
}

// ── Selective preloading strategy: only preload marked routes after idle time
@Injectable({ providedIn: 'root' })
export class SelectivePreloadingStrategy implements PreloadingStrategy {
  private preloading = false;

  constructor(private ngZone: NgZone) {
    // Start preloading after 2s of idle (via requestIdleCallback if available)
    if (typeof requestIdleCallback !== 'undefined') {
      this.ngZone.runOutsideAngular(() => {
        requestIdleCallback(() => {
          this.preloading = true;
        }, { timeout: 2000 });
      });
    } else {
      // Fallback: use timer
      setTimeout(() => { this.preloading = true; }, 2000);
    }
  }

  preload(route: Route, load: () => Observable<any>): Observable<any> {
    // Only preload if route is marked and preloading window has started
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
