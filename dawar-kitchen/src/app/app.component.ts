import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { filter, take } from 'rxjs/operators';
import { HeaderComponent, FooterComponent } from './layout';
import { AnimatedBackgroundComponent, ToastComponent, SplashScreenComponent, CookieConsentComponent } from './shared/components';
import { CartDrawerComponent } from './features/checkout/components/cart-drawer/cart-drawer.component';
import { CommonModule } from '@angular/common';
import { LanguageService } from './shared/services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    AnimatedBackgroundComponent,
    ToastComponent,
    CartDrawerComponent,
    SplashScreenComponent,
    CookieConsentComponent
  ],
  template: `
    <!-- Splash screen — fades out on first NavigationEnd, removed after transition -->
    <app-splash-screen
      *ngIf="splashVisible()"
      [visible]="pageReady()"
      (transitionend)="onSplashTransitionEnd()">
    </app-splash-screen>

    <div class="relative min-h-screen" [class.nn-page-ready]="pageReady()">
      <app-animated-background [zIndex]="'-z-50'"></app-animated-background>
      <app-header></app-header>
      <main class="nn-page-content">
        <router-outlet></router-outlet>
      </main>
      <app-footer></app-footer>
      <app-toast></app-toast>
      <app-cart-drawer></app-cart-drawer>
      <app-cookie-consent></app-cookie-consent>
    </div>
  `,
  styles: [`
    /* App shell is invisible until first route resolves */
    .nn-page-content {
      opacity: 0;
      transition: opacity 0.3s ease;
    }
    .nn-page-ready .nn-page-content {
      opacity: 1;
    }
  `]
})
export class AppComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly languageService = inject(LanguageService);

  /** true → splash component is mounted in the DOM */
  splashVisible = signal(true);
  /** true → splash starts fading out (passed as [visible] input) */
  pageReady = signal(false);

  ngOnInit(): void {
    this.router.events
      .pipe(
        filter(e => e instanceof NavigationEnd),
        take(1)
      )
      .subscribe(() => {
        window.scrollTo({ top: 0, behavior: 'instant' as ScrollBehavior });
        requestAnimationFrame(() => {
          this.pageReady.set(true);
          // Remove splash from DOM after fade-out transition (450 ms + buffer)
          setTimeout(() => this.splashVisible.set(false), 500);
        });
      });
  }

  /** Safety valve: also remove splash if CSS transitionend fires first */
  onSplashTransitionEnd(): void {
    this.splashVisible.set(false);
  }
}
