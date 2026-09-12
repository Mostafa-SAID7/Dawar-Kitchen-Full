import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';

/**
 * Attaches the Supabase JWT Bearer token to every request
 * going to our own API. Requests to third-party URLs are untouched.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  // Only attach token to our own API
  const isOwnApi = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api');
  if (!token || !isOwnApi) {
    return next(req);
  }

  const authReq = req.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  });
  return next(authReq);
};
