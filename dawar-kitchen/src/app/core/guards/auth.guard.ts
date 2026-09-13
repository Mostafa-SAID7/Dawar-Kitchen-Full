import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../auth/auth.service';

/**
 * Route guard — redirects unauthenticated users to /login.
 * Apply to any route that requires authentication.
 */
export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) {
    return true;
  }

  // Use state.url instead of location.pathname for SSR safety
  router.navigate(['/login'], { queryParams: { next: state.url } });
  return false;
};
