import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ToastService } from '../../shared/services/toast.service';
import { TranslateService } from '@ngx-translate/core';

/**
 * Global HTTP Error Interceptor
 * - Catches 401 Unauthorized errors and forces a logout
 * - Displays generic toast error messages for server errors
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth      = inject(AuthService);
  const router    = inject(Router);
  const toast     = inject(ToastService);
  const translate = inject(TranslateService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token expired or unauthorized
        auth.logout();
        toast.error(translate.instant('auth.invalidCredentials'));
        router.navigate(['/login']);
      } else if (error.status >= 500) {
        // Server errors
        toast.error(translate.instant('common.error'));
      }
      return throwError(() => error);
    })
  );
};
