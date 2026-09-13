import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '@core/auth/auth.service';
import { SeoService } from '@shared/services';
import { ToastService } from '@shared/services';
import { AppValidators } from '@shared/validators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, TranslateModule],
  template: `
    <div class="min-h-screen pt-32 pb-16 px-6 flex items-center justify-center bg-[#0a0a0a]">
      <div class="w-full max-w-md p-8 rounded-2xl bg-[#0d0d0d] border border-white/5 shadow-2xl">
        <div class="text-center mb-8">
          <h2 class="font-['Forum'] text-3xl text-white tracking-tight">{{ 'auth.welcomeBack' | translate }}</h2>
          <p class="text-xs text-neutral-400 mt-2">{{ 'auth.signInToAccount' | translate }}</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">
          <div *ngIf="errorMessage"
               class="p-4 rounded-xl bg-red-500/10 border border-red-500/20 text-red-400 text-sm text-center">
            {{ errorMessage }}
          </div>

          <!-- Email -->
          <div class="space-y-2">
            <label class="text-xs font-medium text-neutral-300 tracking-wider uppercase">{{ 'auth.email' | translate }}</label>
            <input type="email" formControlName="email"
                   [placeholder]="'auth.email' | translate"
                   class="nn-input" />
            <div *ngIf="f['email'].touched && f['email'].errors" class="text-xs text-red-400">
              <span *ngIf="f['email'].errors['required']">{{ 'auth.emailRequired' | translate }}</span>
              <span *ngIf="f['email'].errors['email']">{{ 'auth.emailInvalid' | translate }}</span>
            </div>
          </div>

          <!-- Password -->
          <div class="space-y-2">
            <label class="text-xs font-medium text-neutral-300 tracking-wider uppercase">{{ 'auth.password' | translate }}</label>
            <input type="password" formControlName="password" placeholder="••••••••" class="nn-input" />
            <div *ngIf="f['password'].touched && f['password'].errors" class="text-xs text-red-400">
              <span *ngIf="f['password'].errors['required']">{{ 'auth.passwordRequired' | translate }}</span>
            </div>
          </div>

          <!-- Submit -->
          <button type="submit" [disabled]="loading"
                  class="w-full py-3.5 text-sm font-medium text-white bg-[#C65A1E] rounded-xl hover:bg-[#a84915]
                         disabled:opacity-50 transition-all duration-300 flex items-center justify-center gap-2">
            <span *ngIf="loading" class="animate-spin rounded-full h-4 w-4 border-2 border-white border-t-transparent"></span>
            {{ loading ? ('auth.signingIn' | translate) : ('auth.login' | translate) }}
          </button>
        </form>

        <div class="mt-6 text-center text-xs text-neutral-500">
          {{ 'auth.noAccount' | translate }}
          <a routerLink="/register" class="text-[#C65A1E] hover:underline">{{ 'auth.signUp' | translate }}</a>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly fb        = inject(FormBuilder);
  private readonly auth      = inject(AuthService);
  private readonly router    = inject(Router);
  private readonly seo       = inject(SeoService);
  private readonly translate = inject(TranslateService);

  form: FormGroup;
  errorMessage: string | null = null;
  loading = false;

  constructor() {
    this.seo.setLogin();
    this.form = this.fb.group({
      email:    ['', AppValidators.email],
      password: ['', AppValidators.required]
    });
  }

  get f() { return this.form.controls; }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.errorMessage = null;
    const { email, password } = this.form.value;
    this.auth.login(email, password).subscribe({
      next: (success: any) => {
        this.loading = false;
        if (success) { this.router.navigate(['/']); }
        else { this.errorMessage = this.translate.instant('auth.invalidCredentials'); }
      },
      error: () => {
        this.loading = false;
        this.errorMessage = this.translate.instant('auth.loginErrorMsg');
      }
    });
  }
}
