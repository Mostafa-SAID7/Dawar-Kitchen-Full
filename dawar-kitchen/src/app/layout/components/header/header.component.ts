import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { CheckoutService } from '@features/checkout/services/checkout.service';
import { CartService } from '@features/checkout/services/cart.service';
import { AuthService } from '@core/auth/auth.service';
import { ThemeService, LanguageService } from '@shared/services';
import { AuthModalComponent } from '../../auth-modal/auth-modal.component';
import { ClickOutsideDirective } from '@shared/directives';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, AuthModalComponent, TranslateModule, ClickOutsideDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {
  mobileMenuOpen = false;
  userMenuOpen   = signal(false);
  authModalOpen  = signal(false);

  readonly cart  = inject(CartService);
  readonly auth  = inject(AuthService);
  readonly theme = inject(ThemeService);
  readonly language = inject(LanguageService);
  private readonly router = inject(Router);

  currentLang$ = this.language.getCurrentLanguage();

  get userInitial(): string {
    const email = this.auth.userEmail();
    return email ? email.charAt(0).toUpperCase() : '?';
  }

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
    if (this.mobileMenuOpen) this.userMenuOpen.set(false);
  }

  toggleUserMenu() {
    this.userMenuOpen.update(v => !v);
  }

  openAuthModal() {
    this.authModalOpen.set(true);
    this.mobileMenuOpen = false;
    this.userMenuOpen.set(false);
  }

  closeAuthModal() {
    this.authModalOpen.set(false);
  }

  logout() {
    this.auth.logout();
    this.userMenuOpen.set(false);
    this.router.navigate(['/']);
  }
}
